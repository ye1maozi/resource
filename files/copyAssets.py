# -*- coding: utf-8 -*-
import sys
import os
import os.path
import shutil
import re
#####
##asset 不需要替换的文件
##添加文件/src/main/unreplace.txt
##unreplace内容:每行为文件／文件夹名
##
######
#python copyAssets 目录 (sdk名)
# 复制assets
#
andRoot_zc = r"/Users/miao/zc2017/sdk/zc2017/trunk/android"
andRoot_jldj = r"/Users/miao/zc2017/sdk/jldj/trunk/android"
tag = "proj."
pfilter = "proj.and"
srcPath = "/src/main/"
# txt = srcPath + "unreplace.txt"
tmpPath = os.getcwd()  + "/tmp10086"


nofindFile = []

#替换单个资源
def replaceOne(src,des,n):
	print("\n++++++++++替换" + n +".  start++++++++++")
	print "src " + src
	print "des " + des 
	print "\n"

	# shutil.copy2(src, des)
	os.system('cp -r ' + src+ " "+ des)
	print("\n+++++++++++" + n +".  end++++++++++\n")
	pass

#替换assets文件夹
def copyOne(src,des,n):
	print("\n++++++++++" + n +".  start++++++++++")
	print "src " + src
	print "des " + des 
	print "\n"

	localPath = os.getcwd() #当前目录. default download

	# tp = ""
	# tfile = []

	#
	#不需要删除的文件 unreplace.txt
	# if os.path.exists(des + txt):
	# 	#需要tmp文件
	# 	tp = tmpPath + "/"+n
	# 	if not os.path.exists(tp):
	# 		os.mkdir(tp)
	# 		pass
	# 	pass
	# 	with open( des + txt) as unpTxt:

	# 		f = unpTxt.read()
	# 		ff = f.split("\n")
	# 		print ff
	# 		for file in ff:
	# 			file = file.replace("\r","")
	# 			if file != "":
	# 				refile = des + srcPath + "assets/" + file
	# 				print "临时缓存 " + refile
	# 				if os.path.exists(refile):
	# 					if os.path.isdir(refile):
	# 						tfile.append( file)
	# 						shutil.copytree(refile,tp+ "/" + file)
	# 						pass
	# 					else:
	# 						tfile.append( file)
	# 						shutil.copyfile(refile,tp+ "/" + file)
	# 					pass
	# 				else:
	# 					print "没有找到文件。。" + file
	# 					nofindFile.append("没有找到文件.. in sdk: " + n + "   文件名:"+ file)
	# 					pass
	# 				pass
				

			
	# 		unpTxt.close()
	# 		pass

	#删除

	mkDirName = des + srcPath + "assets"

	
	if os.path.exists(mkDirName):
		shutil.rmtree(mkDirName)
		# os.mkdir(mkDirName)
		pass
	else:
		# os.mkdir(mkDirName)
		pass


	#复制 
	print "*******复制assets src" + src
	print "*******复制assets mkDirName " +mkDirName
	print "\n"
	shutil.copytree(src,mkDirName)

	backDir = des + srcPath + "assetBack"
	backDir2 = des + srcPath + "assets_bak"
	backDir3 = des + srcPath + "assetback"
	print "备份文件夹.." + backDir
	if os.path.exists(backDir):
		backDir = backDir
	elif os.path.exists(backDir2):
		backDir = backDir2
	elif os.path.exists(backDir3):
		backDir = backDir3
		pass
	#复制
	if os.path.exists(backDir):
		# os.system('cp -r ' + backDir+ " "+ mkDirName)
		backs = os.listdir(backDir)
		for bk in backs:
			if(os.path.isdir(backDir + '/' + bk)): 
				shutil.copytree(backDir + "/" + bk,mkDirName+ "/" + bk)
				pass
			else:
				shutil.copyfile(backDir + "/" + bk,mkDirName+ "/" + bk)
				pass

		print "存在备份文件夹.." + backDir
		pass
	# if tp != "":
	# 	for tf in tfile:
	# 		if os.path.exists(tp + "/" + tf) and not os.path.exists(mkDirName+ "/" + tf):
	# 			if os.path.isdir(tp + "/" + tf):
	# 				print "还原文件" + tf
	# 				shutil.copytree(tp + "/" + tf,mkDirName+ "/" + tf)
	# 				pass
	# 			else:
	# 				print "还原文件" + tf
	# 				shutil.copyfile(tp + "/" + tf,mkDirName+ "/" + tf)
	# 			pass
	# 		else:
	# 			# print "没有找到文件。。" + file
	# 			pass
		
	# 	pass

	print("\n+++++++++++" + n +".  end++++++++++\n")
	pass	


def copyAssets(srcAssets,desAssets):

	#替换目标目录
	if srcAssets.find("zc2017") != -1:
		andRoot = andRoot_zc
		pass
	else:
		andRoot = andRoot_jldj	

	print "目标 。。。。 " + andRoot
	localPath = os.getcwd() #当前目录. default download

	print srcAssets
	print desAssets
	print "------------"
	curSrc = localPath + "/" + srcAssets
	if not os.path.isdir(curSrc):
		print "不是文件夹"
		return
		pass
	if not os.path.exists(src):
		print("目标文件夹不存在" + src)
		return
		pass

	t = curSrc.split("/")
	# print t
	tt = t[len(t)-1]
	t[len(t)-1] = "assets"
	chSrc = "/".join(t)
	
	print "复制 " + curSrc
	print "修改 " + chSrc
	 # 返回一个列表
	if os.path.exists(chSrc)  :
		if tt != "assets" :
			shutil.rmtree(chSrc)
			shutil.copytree(curSrc,chSrc)
		pass
	else:
		shutil.copytree(curSrc,chSrc)



	copyall = True

	if len(os.listdir(curSrc)) < 1000:
		copyall = False
		pass
	#遍历
	names = os.listdir(andRoot) 
	

	# os.rename(curSrc, chSrc)

	#需要tmp文件
	# if not os.path.exists(tmpPath):
	# 	os.mkdir(tmpPath)
	# 	pass
	# else:
	# 	shutil.rmtree(tmpPath)
	# 	os.mkdir(tmpPath)
	# pass

	for name in names:
		if(os.path.isdir(andRoot + '/' + name)): 
			#文件夹
			# print "目录 　"+name
			if name.find(tag) != -1 and name != pfilter:
				print "sdk目录 " + name
				if desAssets == "" or desAssets == name:
					
					print "##################"
					print "操作目录    " + name

					if copyall:
						copyOne(chSrc,andRoot + "/" +name,name)
					else:
						replaceOne(chSrc,andRoot + "/" +name,name)
						pass
					
					print "##################"
					pass

				pass
		pass

	#删除
	# shutil.rmtree(tmpPath)
	if  tt != "assets":
		shutil.rmtree(chSrc)
		pass

	# if len(nofindFile) > 0:
	# 	for x in nofindFile:
	# 		print x
	# 		pass
	# 	pass
	print "copy end ..."


if __name__ == '__main__':
	print os.getcwd()
	print len(sys.argv)
	if len(sys.argv) < 2:
		print("要复制的目录")
	else:
		# sdk
		src = sys.argv[1]
		des = ""
		if len(sys.argv) > 2:
			##目标
			des = "proj." + sys.argv[2]
		
		copyAssets(src,des)