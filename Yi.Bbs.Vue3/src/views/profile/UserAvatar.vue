<template>
  <div class="user-info-head" @click="editCropper()">
    <img :src="options.img" title="点击上传头像" class="img-circle img-lg" />
  </div>
  <el-dialog
    :title="title"
    v-model="open"
    width="800px"
    append-to-body
    @opened="modalOpened"
    @close="closeDialog"
  >
    <el-row>
      <el-col :xs="24" :md="12" :style="{ height: '350px' }">
        <vue-cropper
          ref="cropper"
          :img="options.img"
          :info="true"
          :autoCrop="options.autoCrop"
          :autoCropWidth="options.autoCropWidth"
          :autoCropHeight="options.autoCropHeight"
          :fixedBox="options.fixedBox"
          @realTime="realTime"
          v-if="visible"
        />
      </el-col>
      <el-col :xs="24" :md="12" :style="{ height: '350px' }">
        <div class="avatar-upload-preview">
          <img :src="options.previews.url" :style="options.previews.img" />
        </div>
      </el-col>
    </el-row>
    <br />
    <el-row v-show="!isDisable">
      <el-col :lg="{ span: 1, offset: 2 }" :md="2">
        <el-upload
          action="#"
          :http-request="requestUpload"
          :show-file-list="false"
          :before-upload="beforeUpload"
        >
          <el-button>
            本地选择
            <el-icon class="el-icon--right">
              <Upload />
            </el-icon>
          </el-button>
        </el-upload>
      </el-col>
      <el-col :lg="{ span: 1, offset: 3 }" :md="2">
        <el-button @click="handleSelectOnline"
          >在线选择<el-icon class="el-icon--right"> <Upload /> </el-icon>
        </el-button>
      </el-col>
    </el-row>
    <el-row style="margin-top: 10px" v-show="!isDisable">
      <el-col :lg="{ span: 1, offset: 2 }" :md="2">
        <el-button icon="Plus" @click="changeScale(1)"></el-button>
      </el-col>
      <el-col :lg="{ span: 1, offset: 1 }" :md="2">
        <el-button icon="Minus" @click="changeScale(-1)"></el-button>
      </el-col>
      <el-col :lg="{ span: 1, offset: 1 }" :md="2">
        <el-button icon="RefreshLeft" @click="rotateLeft()"></el-button>
      </el-col>
      <el-col :lg="{ span: 1, offset: 1 }" :md="2">
        <el-button icon="RefreshRight" @click="rotateRight()"></el-button>
      </el-col>
      <el-col :lg="{ span: 2, offset: 8 }" :md="2">
        <el-button type="primary" @click="uploadImg()">上传</el-button>
      </el-col></el-row
    >
  </el-dialog>
  <!-- 选择已有弹窗 -->
  <el-dialog
    v-model="isOnlineVisible"
    title="在线选择"
    append-to-body
    width="800px"
  >
    <div class="imageList">
      <template v-for="(item, index) in iconList">
        <el-image
          style="width: 100px; height: 100px"
          :src="item"
          :fit="fit"
          :class="{ selected: index === selectedImageIndex, imageBox: true }"
          @click="selectImage(index)"
        />
      </template>
    </div>
    <template #footer>
      <span class="dialog-footer">
        <el-button @click="isOnlineVisible = false">取消</el-button>
        <el-button type="primary" @click="confirmImage"> 确认 </el-button>
      </span>
    </template>
  </el-dialog>
</template>

<script setup>
import "vue-cropper/dist/index.css";
import { ref, reactive, onMounted,computed ,watch} from "vue";
import { VueCropper } from "vue-cropper";
import { upload } from "@/apis/fileApi";
import { updateUserIcon } from "@/apis/userApi";
import { getIconList } from "@/apis/settingApi";
import useUserStore from "@/stores/user";
import axios from "axios";

const props = defineProps({
  user: {
    type: Object,
    default:{icon:"/acquiesce.png"}
  },
  isDisable:{
    type:Boolean,
    default:true
  }
});
const userStore=useUserStore();
const cropper = ref(null);

const open = ref(false);
const visible = ref(false);
const title = ref("修改头像");

//图片裁剪数据
const options = reactive({
  img: props.user.icon, // 裁剪图片的地址
  autoCrop: true, // 是否默认生成截图框
  autoCropWidth: 200, // 默认生成截图框宽度
  autoCropHeight: 200, // 默认生成截图框高度
  fixedBox: true, // 固定截图框大小 不允许改变
  previews: {}, //预览数据
});

/** 编辑头像 */
function editCropper() {
  open.value = true;
}
/** 打开弹出层结束时的回调 */
function modalOpened() {
  visible.value = true;
}
/** 覆盖默认上传行为 */
function requestUpload() {}
/** 向左旋转 */
function rotateLeft() {
  cropper.value.rotateLeft();
}
/** 向右旋转 */
function rotateRight() {
  cropper.value.rotateRight();
}
/** 图片缩放 */
function changeScale(num) {
  num = num || 1;
  cropper.value.changeScale(num);
}
/** 上传预处理 */
function beforeUpload(file) {
  if (file.type.indexOf("image/") == -1) {
    ElMessage.error("文件格式错误，请上传图片类型,如：JPG，PNG后缀的文件。");
  } else {
    console.log(file, "file");
    const reader = new FileReader();
    reader.readAsDataURL(file);
    reader.onload = () => {
      options.img = reader.result;
    };
  }
}
/** 上传图片 */
async function uploadImg() {
  await cropper.value.getCropBlob(async (data) => {
    let formData = new FormData();
    formData.append("file", data);
    const response = await upload(formData);
    open.value = false;
    options.img =
      import.meta.env.VITE_APP_BASEAPI + "/file/" + response.data[0].id;
    userStore.icon = options.img;
    const iconResponse = await updateUserIcon(response.data[0].id);
    if (iconResponse.status == 200) {
      ElMessage({
        type: "success",
        message: "头像更新成功",
      });
    }
  });
}
/** 实时预览 */
function realTime(data) {
  options.previews = data;
}
/** 关闭窗口 */
function closeDialog() {
  options.img = userStore.icon;
  options.visible = false;
}

// 在线选择相关逻辑
const saveOnlineImage = async (imageUrl) => {
  try {
    // 发起网络请求下载在线图片
    const response = await axios.get(imageUrl, {
      responseType: "blob", // 设置响应类型为blob
    });
    // 创建blob对象
    const blob = new Blob([response.data], {
      type: response.headers["content-type"],
    });

    // 创建File对象，设置文件名为当前时间戳
    const file = new File([blob], `${Date.now()}.jpg`, {
      type: response.headers["content-type"],
    });

    // 可以将file对象传递给其他处理逻辑或进行文件上传操作
    return file;
  } catch (error) {
    console.error("下载图片失败:", error);
  }
};
const iconList = ref([]);
const isOnlineVisible = ref(false);
const handleSelectOnline = () => {
  isOnlineVisible.value = true;
};
const selectedImageIndex = ref(null);
const selectImage = (index) => {
  selectedImageIndex.value = index;
};
const confirmImage = async () => {
  if (selectedImageIndex !== null) {
    // 在预览区域中显示选择的图片
    const selectedImage = iconList.value[selectedImageIndex.value];
    const file = await saveOnlineImage(selectedImage);
    beforeUpload(file);
  }
  selectedImageIndex.value = null;
  isOnlineVisible.value = false;
};

watch(
  () => props,
   (val,oldValue) => {
    const userIcon=props.user.icon;
    //console.log(userIcon,"userIcon")
    options.img=userIcon == "" || userIcon == null
                ? "/acquiesce.png"
                : import.meta.env.VITE_APP_BASEAPI + "/file/" + userIcon;
          
  },
  {immediate:true,deep:true}
);
onMounted(async () => {
  const { data: iconListData } = await getIconList();
  iconList.value = iconListData.map(
    (item) => import.meta.env.VITE_APP_BASEAPI + "/" + item
  );
});
</script>

<style lang="scss" scoped>
.user-info-head {
  position: relative;
  display: inline-block;
  height: 120px;
}

.user-info-head:hover:after {
  content: "+";
  text-align: center;
  position: absolute;
  left: 0;
  right: 0;
  top: 0;
  bottom: 0;
  color: #eee;
  background: rgba(0, 0, 0, 0.5);
  font-size: 24px;
  font-style: normal;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  cursor: pointer;
  line-height: 110px;
  border-radius: 50%;
}

.img-circle {
  border-radius: 50%;
}

.img-lg {
  width: 120px;
  height: 120px;
}

.avatar-upload-preview {
  position: absolute;
  top: 50%;
  transform: translate(50%, -50%);
  width: 200px;
  height: 200px;
  border-radius: 50%;
  box-shadow: 0 0 4px #ccc;
  overflow: hidden;
}

/* 添加选中图片的边框样式 */
.imageBox {
  margin: 10px;
}
.selected {
  padding: 5px;
  border: 2px solid #40a9ff;
}
</style>
