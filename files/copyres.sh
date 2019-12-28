#! /bin/bask

echo "输入的参数格式 zjze_axxx.zip androi的工程明(nosdk)"
echo ">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>"
echo "."
echo ".."
echo "..."

#工程目录ios
targetIOS="/Users/miao/zc2017/zjzrSDK/zjzr2_client/ios/proj.ios/"
targetRaw="/Data/"
#工程目录android
targetANDROID="/Users/miao/zc2017/zjzrSDK/zjzr2_client/android/"
flagANDROID="proj."
targetPath="/src/main/"


echo "输入的参数 ... "
for var in "$*"
do
	echo "$var"
done 
echo "检查参数..."
#路径查看
allPath=$1
allUrl=$(echo ${allPath%/*})
tarZip=$1
# echo $allUrl
if [[ $allUrl != "" ]]; then
	cd $allUrl
	echo "当前目录切换到" 
	tarZip=$(echo ${allPath##*/})
	pwd
	
fi

zipTargetPath=${tarZip%.*}

echo "操作文件:"$zipTargetPath

if [ ! -f "$tarZip" ]; then
  echo "$1 not exist"
  exit
fi

isIos=$(echo $tarZip |grep "appstore")
isAndroid=$(echo $tarZip |grep "android")

if [[ "$isIos" != "" ]]; then
	echo "ios project"
	targetRoot="$targetIOS$targetRaw" 
	targetDirName="Raw"
fi

if [[ "$isAndroid" != "" ]]; then
	echo "android project"
	targetRoot="$targetANDROID$flagANDROID$2$targetPath" 
	targetDirName="assets"
fi

if [[ ! -d "$targetRoot" ]]; then
	echo $targetRoot"目录不存在"
	exit 
fi

echo "输入文件 : "$tarZip
echo "输出目录 : "$targetRoot

echo "参数检查结束"

echo "删除原来目录"
rm -rf $zipTargetPath
echo "upzip start..."
unzip -o $tarZip -d $zipTargetPath

echo "再次解压" 
cd $zipTargetPath
childZip=""
for i in `ls | xargs`; do
 
 	isZipTar=$(echo $i |grep "zip")

 	if [[ "$isZipTar" != "" ]]; then
 		#statements
 		childZip=$i
 	fi
done
if [[ "$childZip" != "" ]]; then
	zipTargetPath=${childZip%.*}
	unzip -o $childZip -d $zipTargetPath


	echo "解压完成"

	echo ${zipTargetPath}"/"${zipTargetPath}
	# tmp="./Raw"
	echo "删除原来目录"
	rm -rf $targetRoot$targetDirName
	echo "输出目录 : "$targetRoot
	echo "输出文件夹 : "$targetDirName

	cp -r -f ${zipTargetPath}"/"${zipTargetPath} $targetRoot$targetDirName


	# todo 存在备份文件 需要加入
fi

open $targetRoot
echo "done ..."
# if [[ !-f "" ]]; then
# 	#statements
# fi