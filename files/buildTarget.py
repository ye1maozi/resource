# -*- coding: utf-8 -*-
import sys
import os
import os.path
import shutil
import re
import time
import datetime

##python buildTarget.py baiduSdk
#需要修改storeFile 文件目录
#需要配置gradle环境变量

## 自动提升version
## 签名配置
## 使用gradle打包
## 重命名apk


andRoot = r"../../android/"
zcry = "zydxc"
manifestFile = r"/src/main/AndroidManifest.xml"
buildFile = r"/build.gradle"


versionCode = "nil"
versionName = "nil"
packageName = "nil"
proname = ""
renamePro = ""
lastCwd = ""
needInstall = False

# 签名配置
signInfo = """
	signingConfigs {
	    debug {

	    }
	    //配置签名的关键信息
	    release {
	        storeFile file("/Users/miao/zc2017/sdk/zc2017/trunk/android/certificates/jldj.jks") //修改本机地址
	        storePassword "12345678"
	        keyAlias "jldj"
	        keyPassword "12345678"
	        v1SigningEnabled true  
        	v2SigningEnabled true  
	    }
	}
"""

# 版本号递增
def addVersiokn(proRoot):
	global versionCode
	global versionName
	global packageName
	
	mfile = proRoot + manifestFile
	if not os.path.exists(mfile):
		print "manifest不存在 .. " + mfile
		return False

	
	with open( mfile) as imlFile:
			f = imlFile.read()
			# print f
			## versionName
			match = re.search(r'android:versionName="(.*?)"',f)	 
	 		print "-------re----"
			if match:
				# 使用Match获得分组信息
				# print match.group()
				print match.groups()[0]
				vcode = match.groups()[0]
				vsplit = vcode.split(".")
				l = len(vsplit)
				v1 = "1"
				v2 = "0"
				v3 = "0"
				if l > 0:
					v1 = vsplit[0]
				
				if l > 1:
					v2 = vsplit[1]
					
				if l > 2:
					v3 = vsplit[2]

				v3 = int(v3) + 1

				versionName =  '{}.{}.{}'.format(v1,v2,v3) #v1 + "." + v2 + "." + v3
				print "versionCode .." + versionName
				

			# vcode = 
			## versionCode 
			t = time.time()
			versionCode =  str((int(t)))
			print "versionCode.. "+versionCode
			## packageName
			match = re.search(r'package="(.*?)"',f)	 
			if match:
				# 使用Match获得分组信息
				# print match.group()
				packageName = match.groups()[0]
				print "packageName .. " +packageName
			

			f = re.sub('android:versionName="(.*?)"','android:versionName="' + versionName + '"',f)
			f = re.sub('android:versionCode="(.*?)"','android:versionCode="' + versionCode + '"',f)
			imlFile = open(mfile,'w')
			imlFile.write(f)
			imlFile.close()
	return True
	pass


# 替换签名
def findSign(proRoot):

	bfile = proRoot + buildFile
	if not os.path.exists(bfile):
		print "build.gradle不存在 .. " + bfile
		return False



	with open( bfile) as imlFile:
		f = imlFile.read()

		match = re.search(r'signingConfigs',f)
		needWrite = False
		if not match:
			needWrite = True
			f = re.sub('buildTypes',signInfo +os.linesep + "	buildTypes",f)
			pass
		
		match = re.search(r'signingConfig signingConfigs.release',f)
		if not match:
			needWrite = True
			f = re.sub('proguardFiles','signingConfig signingConfigs.release' + os.linesep + "	proguardFiles",f)
			pass

		# print f

		if needWrite:
			imlFile = open(bfile,'w')
			imlFile.write(f)
			imlFile.close()
			pass


	return True
	pass


# 使用gradle打包
def gradleBuild(proRoot):
	#通过gradle
	print proRoot
	os.chdir(proRoot)
	print os.getcwd()
	os.system("gradle clean assembleRelease")
	pass


# 重命名
def renameApk():
	print "输出文件。。。"
	buildRoot = "/build/outputs/apk/"
	releaseFlag = "release"
	outRoot = os.getcwd() + buildRoot
	names = os.listdir(outRoot)
	t = time.time()
	print t
	for name in names:
		if( not os.path.isdir(outRoot + '/' + name)):
			fileapk = outRoot + '/' + name
			
			if t < os.path.getmtime(fileapk) + 60*30: ##  30分钟内
				#print os.path.getmtime(fileapk)
				fileInfo = os.path.splitext(name)
				# 项目名-渠道--relese-versionName-versionCode-日期.apk
				day =   (datetime.datetime.now().strftime('%Y.%m.%d'))
				if renamePro != None and renamePro != "":
					newapk = zcry + "-" + renamePro  + "-release-" + versionName + "-" +versionCode + "-" +day  + fileInfo[1]
				else:
					newapk = zcry + "-" + proname  + "-release-" + versionName + "-" +versionCode + "-" +day  + fileInfo[1]
					pass
				print newapk
				os.rename(fileapk,outRoot + "/" + newapk)
		pass

	os.system("open " + outRoot)
	if needInstall:
		cmdstr = "adb install -r " + outRoot + "/" + newapk
		print cmdstr
		os.system(cmdstr)
		pass

	os.chdir(lastCwd)

	pass


# 其他操作
# 安装。。。
def extOption():


	pass



def mainTask(proName):
	proRoot = andRoot + proName
	if not os.path.exists(proRoot) :
		print "目录不存在 .. " + proRoot
		return
	print "开始... " + proName

	## 修改manifest 
	if not addVersiokn(proRoot):
		return

	## 修改build.gradle
	findSign(proRoot)
	## gradle 
	gradleBuild(proRoot)
	## 重命名apk
	renameApk()

	#其他操作
	extOption()
	pass



## param {1}目标工程 替换名称 是否直接安装 
if __name__ == '__main__':
	print os.getcwd()
	print len(sys.argv)
	if len(sys.argv) < 2:
		print("输入sdk项目名")
	else:
		# sdk
		lenP = len(sys.argv)
		print "长度 " + str(lenP)
		global lastCwd
		lastCwd = os.getcwd()
		global proname
		proname = sys.argv[1]
		if lenP > 2:
			renamePro = sys.argv[2] 
			pass
		if lenP > 3:
			needInstall = True
			pass
		
		
		print "替换名 " + renamePro
		print "是否直接安装" + str(needInstall)
		mainTask("proj." + proname)
