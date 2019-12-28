# 工程绝对路径
PROJECT_PATH="/Workspace/Unity/jldj_client/trunk/ios/proj.ios"
TARGET_NAME="jldj_test"
CODE_SIGN_IDENTITY="iPhone Developer: WENJIA ZHOU (HUKKTBGCR9)"
PROVISIONING_PROFILE="f1436f1d-d5b6-44d9-807d-de36bb86bbd1"
TEAM_ID="T9AV7L8BJN"
PROJECT_CLEAN="no"

# build文件夹路径
BUILD_PATH=${PROJECT_PATH}/build

# cd到工程目录
cd ${PROJECT_PATH}

# 工程配置文件路径
PROJECT_NAME=$(ls | grep xcodeproj | awk -F.xcodeproj '{print $1}')

# 删除bulid目录
if  [ -d ${BUILD_PATH}/ipa-build ];then
    rm -rf ${BUILD_PATH}/ipa-build
    echo clean build_path success.
fi
if  [ -d ${BUILD_PATH}/Release-iphoneos/${TARGET_NAME}.app ];then
    rm -rf ${BUILD_PATH}/Release-iphoneos/${TARGET_NAME}.app
    echo clean app success.
fi

# 清理工程
if [ "${PROJECT_CLEAN}" = "yes" ];then
    xcodebuild clean || exit
fi

#编译工程
xcodebuild  -configuration Release \
-project ${PROJECT_PATH}/${PROJECT_NAME}.xcodeproj \
-target ${TARGET_NAME} \
-sdk iphoneos build \
teamId="${TEAM_ID}"
CODE_SIGN_IDENTITY="${CODE_SIGN_IDENTITY}" \
PROVISIONING_PROFILE="${PROVISIONING_PROFILE}" \
CONFIGURATION_BUILD_DIR=${PROJECT_PATH}/build/Release-iphoneos

if  [ ! -d ${BUILD_PATH}/Release-iphoneos/${TARGET_NAME}.app ];then
    echo "pack failed"
    exit 1
fi

# 打包
cd $BUILD_PATH
mkdir -p ipa-build/Payload
cp -r ./Release-iphoneos/${TARGET_NAME}.app ./ipa-build/Payload/

# 打包IPA
cd ipa-build
zip -r ${PROJECT_NAME}.ipa *

# 输出产品路径
echo ${BUILD_PATH}/ipa-build/${PROJECT_NAME}.ipa

echo "pack succ"
exit 0