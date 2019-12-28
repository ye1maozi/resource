using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine;
using Spine.Unity;
using Spine.Unity.Modules.AttachmentTools;
using LitJson;

public class SpineHelper : MonoBehaviour {

    Skeleton skeleton;
    Skin activeSkin;
    Material sourcesMaterial = null;
    private Dictionary<Slot, List<Attachment>> attachmentTable;
  
    public string SkinName;
    public Sprite ChangeSprite;


    /// <summary>
    /// 注意:图片资源需要设置为可读写
    /// </summary>
    /// <param name="spineObject"></换装的角色>
    /// <param name="changeSkinName"></换装部位名称>
    /// <param name="sprite"></换装资源>
    public void ChangeSpineSkin(GameObject spineObject, string changeSkinName, Sprite sprite)
    {
        Debug.Log("ChangeSpineSkin");
        if (sprite == null)
        {
            sprite = ChangeSprite;
        }

        SkeletonAnimator ator = GetComponent<SkeletonAnimator>();
        skeleton = ator.Skeleton;
        //对应的插槽
        Slot targetSlot = skeleton.FindSlot(changeSkinName);
        if (targetSlot == null)
        {
            Debug.Log("Not Find Slot!");
        }
        else
        {
           
            sourcesMaterial = ator.SkeletonDataAsset.atlasAssets[0].materials[0];
            //克隆一份skin 否则元数据会被修改
            activeSkin =  skeleton.GetClonedSkin("default");
            //需要重新赋值下skin
 			skeleton.SetSkin(activeSkin);
            //RegionAttachment oldAtt = targetSlot.Attachment as RegionAttachment;
       
            int slotIndex = targetSlot.Data.Index;
            //需要替换的部件
        
            Attachment templateAttachment = targetSlot.Attachment;
            //复制部件 使用新的sprite

            Attachment newAttachment = null;
            AtlasRegion atlas = ToAtlasRegionPMAClone(sprite, sourcesMaterial.shader, TextureFormat.RGBA32, false, sourcesMaterial);
            //if (templateAttachment as RegionAttachment != null)
            //{
               
                newAttachment = GetRemappedClone(templateAttachment, atlas, true, false, 1f / sprite.pixelsPerUnit);
            //}
            //else
            //{
            //    var unitsPerPixel = 1f / sprite.pixelsPerUnit;
            //    atlas.rotate = true;
            //    atlas.offsetX = 60;
            //    //atlas.offsetY = 0;

            //    RegionAttachment att = atlas.ToRegionAttachment(changeSkinName, unitsPerPixel);
            //    att.rotation = -90f;
                
            //    newAttachment = att;
            //    //Attachment newAttachment = GetRemappedClone(templateAttachment, atlas, true, false, 1f / sprite.pixelsPerUnit);
            //}
           
            //Attachment newAttachment = templateAttachment.GetRemappedClone(sprite, sourcesMaterial);
            //把skin的部件数据替换
            activeSkin.SetAttachment(slotIndex, templateAttachment.Name, newAttachment);
            //插槽部件数据替换
            targetSlot.Attachment = newAttachment;
           // skeleton.SetAttachment(changeSkinName, templateAttachment.Name);
        }
        
    }
    	public Attachment GetRemappedClone (Attachment o, AtlasRegion atlasRegion, bool cloneMeshAsLinked = true, bool useOriginalRegionSize = false, float scale = 0.01f) {
			var regionAttachment = o as RegionAttachment;
			if (regionAttachment != null) {
				RegionAttachment newAttachment = regionAttachment.GetClone();
				newAttachment.SetRegion(atlasRegion, false);
				if (!useOriginalRegionSize) {
					newAttachment.width = atlasRegion.width * scale;
					newAttachment.height = atlasRegion.height * scale;
				}
				newAttachment.UpdateOffset();
				return newAttachment;
			} else {

                var meshAttachment = o as MeshAttachment;
                if (meshAttachment != null)
                {

                    MeshAttachment newAttachment = cloneMeshAsLinked ? meshAttachment.GetLinkedClone(cloneMeshAsLinked) : meshAttachment.GetClone();
                    //var vertices = newAttachment.vertices;
                    newAttachment.SetRegion(atlasRegion);
                    return newAttachment;
                }
                
			}

			return o.GetClone(true); // Non-renderable Attachments will return as normal cloned attachments.
		}
    static Texture2D ToTexture(Sprite s, bool applyImmediately = true, TextureFormat textureFormat = TextureFormat.RGBA32, bool mipmaps = false)
    {
        var spriteTexture = s.texture;
        Rect r = s.rect;
        var spritePixels = spriteTexture.GetPixels((int)r.x, (int)r.y, (int)r.width, (int)r.height);
        var newTexture = new Texture2D((int)r.width, (int)r.height, textureFormat, mipmaps);
        newTexture.SetPixels(spritePixels);
        Debug.Log("texture width height" + r.width + "   " + r.height);
        if (applyImmediately)
            newTexture.Apply();

        return newTexture;
    }
   void ApplyPMA (Texture2D texture, bool applyImmediately = true) {
			var pixels = texture.GetPixels();
			for (int i = 0, n = pixels.Length; i < n; i++) {
				Color p = pixels[i];
				float a = p.a;
				p.r = p.r * a;
				p.g = p.g * a;
				p.b = p.b * a;
				pixels[i] = p;
			}
			texture.SetPixels(pixels);
			if (applyImmediately)
				texture.Apply();
		}
    public AtlasRegion ToAtlasRegionPMAClone(Sprite s, Shader shader, TextureFormat textureFormat = TextureFormat.RGBA32, bool mipmaps = false, Material materialPropertySource = null)
    {
        var material = new Material(shader);
        if (materialPropertySource != null)
        {
            material.CopyPropertiesFromMaterial(materialPropertySource);
            material.shaderKeywords = materialPropertySource.shaderKeywords;
        }

        material.renderQueue = material.renderQueue + 1;
        var tex = ToTexture(s,false, textureFormat, mipmaps);
        ApplyPMA(tex,true);

        tex.name = s.name + "-pma-";
        material.name = tex.name + shader.name;

        material.mainTexture = tex;
        var page = ToSpineAtlasPage(material);

      //  s.ToAtlasRegion(true);
        var region = CreateRegion(tex, material);// ToAtlasRegion(s, true);

       // region.page = page;

        return region;
    }
    AtlasPage ToSpineAtlasPage( Material m)
    {
        var newPage = new AtlasPage
        {
            rendererObject = m,
            name = m.name
        };

        var t = m.mainTexture;
        if (t != null)
        {
            newPage.width =   t.width;
            newPage.height =   t.height;
        }

        return newPage;
    }
    static float InverseLerp(float a, float b, float value)
    {
        return (value - a) / (b - a);
    }

     AtlasRegion ToAtlasRegion ( Sprite s, bool isolatedTexture = false) {
			var region = new AtlasRegion();
			region.name = s.name;
			region.index = -1;
			region.rotate = s.packed && s.packingRotation != SpritePackingRotation.None;

			// World space units
			Bounds bounds = s.bounds;
			Vector2 boundsMin = bounds.min, boundsMax = bounds.max;

			// Texture space/pixel units
			Rect spineRect = s.rect;//.SpineUnityFlipRect(s.texture.height);

            region.width =  (int)spineRect.width;
            region.originalWidth =  (int)spineRect.width;
            region.height =  (int)spineRect.height;
            region.originalHeight = (int)spineRect.height;
			region.offsetX = spineRect.width * (0.5f - InverseLerp(boundsMin.x, boundsMax.x, 0));
			region.offsetY = spineRect.height * (0.5f - InverseLerp(boundsMin.y, boundsMax.y, 0));

			if (isolatedTexture) {
                Debug.Log("1111111111111111111111");
				region.u = 0;
				region.v = 1;
				region.u2 = 1;
				region.v2 = 0;
				region.x = 0;
				region.y = 0; 

			}
           

			return region;
		}
    AtlasRegion CreateRegion(Texture2D texture,Material m)
    {
        AtlasRegion region = new AtlasRegion();
        region.width =  texture.width;
        region.height = texture.height;
        region.originalHeight =  texture.height;
        region.originalWidth =  texture.width;
        region.rotate = false;
        region.u = 0;
        region.v = 1;
        region.u2 = 1;
        region.v2 = 0;
        region.x = 0;
        region.y = 0; 

        region.page =   new AtlasPage
        {
            rendererObject = m,
            name = m.name
        };
        region.page.name = texture.name;
        region.page.width = texture.width;
        region.page.height = texture.height;
        region.page.uWrap = TextureWrap.ClampToEdge;
        region.page.vWrap = TextureWrap.ClampToEdge;
        return region;
    }
        
    void UpdateAttachments()
    {

        SkeletonAnimator ator = GetComponent<SkeletonAnimator>();
        if (ator == null)
        {
            ISkeletonAnimation ani = GetComponent<SkeletonAnimation>();
            if (ani == null)
            {
                Debug.Log("SpineObject Not Find SkeletonAnimation!");
            }
            else
            {
                skeleton = ani.Skeleton;
            }
        }
        else
        {
            skeleton = ator.Skeleton;
        }
        if (skeleton == null)
        {
            Debug.LogError("SpineObject Not Find skeleton!");
        }
        else
        {
            attachmentTable = new Dictionary<Slot, List<Attachment>>();
        }

        //skeleton = skeletonRenderer.skeleton;
        Skin defaultSkin = skeleton.Data.DefaultSkin;
        Skin skin = skeleton.Skin ?? defaultSkin;
        bool notDefaultSkin = skin != defaultSkin;
        //skeleton.data.slots
        attachmentTable.Clear();
        for (int i = skeleton.Slots.Count - 1; i >= 0; i--)
        {
            var attachments = new List<Attachment>();
            attachmentTable.Add(skeleton.Slots.Items[i], attachments);
            skin.FindAttachmentsForSlot(i, attachments); // Add skin attachments.
            if (notDefaultSkin) defaultSkin.FindAttachmentsForSlot(i, attachments); // Add default skin attachments.
        }

        activeSkin = skeleton.Skin;
    }

    
    void OnGUI()
    {

        //if (GUI.Button(new Rect(10,10,100,20),"click"))
        //{
        //    this.ChangeSpineSkin(this.gameObject, this.SkinName, this.ChangeSprite);
        //}


        //if (attachmentTable != null)
        //{
        //    SlotTable = new Dictionary<string, Dictionary<string, Attachment>>();
        //    Dictionary<string, Attachment> attsTable;

        //    int toolbar = 0;
        //    float y = 10.0f;
        //    int keyIndex = 0;
        //    Dictionary<int, Slot> test = new Dictionary<int, Slot>();
        //    foreach (KeyValuePair<Slot, List<Attachment>> pair in attachmentTable)
        //    {
        //        Slot slot = pair.Key;

        //        GUI.Label(new Rect(50, y, 100, 40), slot.Data.name);
        //        //var toggle1 =
        //        // slot.SetColor(c);


        //        int index = 0;
        //        int len = 100 * pair.Value.Count;
        //        string[] info = new string[pair.Value.Count];
        //        test[keyIndex++] = slot;
        //        if (pair.Value.Count == 1)
        //        {
        //            continue;
        //        }
        //        foreach (var attachment in pair.Value)
        //        {
        //            info[index++] = attachment.Name;
        //            //GUI.Toggle(new Rect(150, y, 40, 40), toggle1, attachment.Name);
        //        }

        //        toolbars[toolbar] = GUI.Toolbar(new Rect(150, y, len, 30), toolbars[toolbar], info);
        //        toolbar++;
        //        y += 25.0f;

        //    }
        //    //第一个测试
        //    for (int i = 0; i < 2; i++)
        //    {
        //        int tool = toolbars[i];

        //        if (i > test.Count)
        //        {
        //            Debug.Log("数组xx " + test.Count);
        //            return;
        //        }
        //        Slot slot = test[i];
        //        List<Attachment> p = attachmentTable[slot];
        //        if (tool <= 0 || tool >= p.Count)
        //        {
        //            Debug.Log("数组" + tool);
        //            return;
        //        }
        //        Attachment att = p[tool];
        //        slot.attachment = att;
        //    }

        //}
        
    }

    private Dictionary<string, Dictionary<string ,Attachment>> SlotTable;
    private Dictionary<string, Slot> slotsDir;
    private string jsonInfo;
    /// <summary>
    /// 当前spine换装的节点信息
    /// </summary>
    public string GetSpineJsonString()
    {
        if (jsonInfo == null)
        {
            generateData();
        }
        return jsonInfo;
    }

    /// <summary>
    /// 换组建
    /// </summary>
    /// <param name="slotName"></param>
    /// <param name="attName"></param>
    public void ChangeSlotWithAttach(string slotName, string attName)
    {
        if (SlotTable == null)
        {
            Debug.LogError("数据未初始");
            return;
        }
        Dictionary<string, Attachment> atts = SlotTable[slotName];
        if (atts != null)
        {
            Attachment attach = atts[attName];
            if (attach != null)
            {
                slotsDir[slotName].attachment = attach;
            }else
            {
                Debug.LogError("attName 不存在 " + attName);
            }
        }
        else
        {
            Debug.LogError("slotname 不存在 " + slotName);
        }
    }

    public void SetSlotColor(string slotName,Color col)
    {
        if (SlotTable == null)
        {
            Debug.LogError("数据未初始");
            return;
        }
        Dictionary<string, Attachment> atts = SlotTable[slotName];
        if (atts != null)
        {
          
            slotsDir[slotName].SetColor(col);
        }
        else
        {
            Debug.LogError("slotname 不存在 " + slotName);
        }
    }
    /// <summary>
    /// 生成数据 slot - attName - att
    /// </summary>
    private void generateData()
    {
        UpdateAttachments();
        if (attachmentTable != null)
        {
            JsonData data = new JsonData();

            slotsDir = new Dictionary<string, Slot>();
            SlotTable = new Dictionary<string, Dictionary<string, Attachment>>();
            Dictionary<string, Attachment> attsTable;

            Dictionary<int, Slot> test = new Dictionary<int, Slot>();
            foreach (KeyValuePair<Slot, List<Attachment>> pair in attachmentTable)
            {
                

                string[] info = new string[pair.Value.Count];
                if (pair.Value.Count == 1)
                {
                    continue;
                }

                attsTable = new Dictionary<string, Attachment>();

                Slot slot = pair.Key;
                string slotName = slot.Data.name;

                data[slotName] = new JsonData();
                JsonData objJson = data[slotName];
                //绑定 name - slot
                slotsDir.Add(slotName, slot);

                foreach (var attachment in pair.Value)
                {
                    objJson.Add(attachment.Name);
                    //绑定name - attachment
                    attsTable.Add(attachment.Name, attachment);
                }
                //绑定 name - name - attachment
                SlotTable.Add(slotName, attsTable);
            }
            jsonInfo = data.ToJson();
        }
        else
        {
            Debug.LogError("attachmentTable == null");
        }
    }

    

}
