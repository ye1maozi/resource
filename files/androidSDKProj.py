# -*- coding: utf-8 -*-
import sys
import os
import os.path
import shutil
import re

# 创建新sdk项目  androidSDKProj.py {sdkName} {?删除原有目录}
# 
# 修改 {projname\src\main\java\com\aoshitang\sdk\xxx.java} 
# 修改 {projname\src\main\java\com\aoshitang\jldj\UnityPlayerActivity.java} 
#

root = r"../android/"
def createPro(proName,delold):
	localPath = os.getcwd() #当前目录
	#  pro.and 复制

	urlPro = root + "proj.and"

	mkDirName = root + proName
	if os.path.exists(mkDirName) :
		if not delold:
			print "error exists dir "+ proName
			return
		shutil.rmtree(mkDirName)
		os.mkdir(mkDirName)
	else:

		print "mkdir "+ proName
		os.mkdir(mkDirName)

	print "copy start ..."
	print "copy .gradle"
	if os.path.exists(urlPro + "/.gradle"):
		shutil.copytree(urlPro + "/.gradle" , mkDirName + "/.gradle")

	if os.path.exists(urlPro + "/gradle"):
		shutil.copytree(urlPro + "/gradle" , mkDirName + "/gradle")
	
	# print "copy lib"
	# shutil.copytree(urlPro + "/libs" , mkDirName + "/libs")
	os.mkdir(mkDirName + "/libs")



	print "copy files ..."
	shutil.copyfile(urlPro + "/local.properties" , mkDirName + "/local.properties")
	shutil.copyfile(urlPro + "/proguard-unity.txt" , mkDirName + "/proguard-unity.txt")
	shutil.copyfile(urlPro + "/proj.and.iml" , mkDirName + "/"+proName+".iml")

	# 默认的配置
	print "copy build.gradle"
	shutil.copyfile(localPath + "/config/build.gradle" , mkDirName + "/build.gradle")
	shutil.copyfile(localPath + "/config/settings.gradle" , mkDirName + "/settings.gradle")

	print "修改配置 .iml "
	with open( mkDirName + "/"+proName+".iml") as imlFile:
		# external.linked.project.id="proj.and"
		#  <orderEntry type="module" module-name="proj.and" exported="" />
		f = imlFile.read()
		f = re.sub('external.linked.project.id="proj.and"',
			'external.linked.project.id="'+ proName+'"',
			f)
		f = re.sub('</component>',
			'<orderEntry type="module" module-name="proj.and" exported="" />\r\n</component>',
			f)
		imlFile = open(mkDirName + "/"+proName+".iml",'w')
		imlFile.write(f)
		imlFile.close()



	#从aoshitang项目复制src
	urlast = root + "proj.aoshitang/"
	print "copy src java"
	shutil.copytree(urlast + "/src/main/java" , mkDirName + "/src/main/java")
	print "copy src res"
	shutil.copytree(urlast + "/src/main/res" , mkDirName + "/src/main/res")


	os.mkdir(mkDirName + "/src/main/assets")
	os.mkdir(mkDirName + "/src/main/jniLibs")
	shutil.copyfile(localPath + "/config/AndroidManifest.xml" , mkDirName + "/src/main/AndroidManifest.xml")

	print "copy end ..."


if __name__ == '__main__':
	print os.getcwd()
	print len(sys.argv)
	if len(sys.argv) < 2:
		print("输入sdk项目名")
	else:
		# sdk
		proname = sys.argv[1]
		delold = len(sys.argv) == 3
		createPro("proj." + proname,delold)