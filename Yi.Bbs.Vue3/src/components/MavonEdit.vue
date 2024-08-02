<template>
  <mavon-editor ref='md' v-model="text" :subfield="true" :codeStyle="props.codeStyle" :ishljs="true"
    :style="{ minHeight: props.height, maxHeight: '100%' }" class="edit" @imgAdd="imgAdd" @change="change">
 
    <!-- 引用视频链接的自定义按钮 -->
    <template v-slot:left-toolbar-after>
      <!--点击按钮触发的事件是打开表单对话框-->


      <el-button @click="fileDialogVisible=true" aria-hidden="true" class="op-icon fa" title="上传文件">
          <el-icon ><FolderChecked /></el-icon>
        </el-button>


      <el-dropdown :hide-on-click='false'>

        
     <el-button aria-hidden="true" class="op-icon fa" title="表情包">
          😊
        </el-button>
        <template #dropdown>

           
              <EmojiPanel @onSelect="onSelectEmoji"/>
        
        </template> 
      </el-dropdown>


      <el-dialog
      :modal=false
      :draggable=true
   
    v-model="fileDialogVisible"
    title="文件上传"
    width="30%"
    :before-close="fileHandleClose"
  >
    <span>选择你的文件：</span>

    <el-upload
    class="upload-demo"
    drag
    :action="fileUploadUrl"
    multiple
    :on-success="onSuccess"
  >
    <el-icon class="el-icon--upload"><upload-filled /></el-icon>
    <div class="el-upload__text">
      可将文件拖拽到这里 <em>点击上传</em>
    </div>
    <template #tip>
      <div class="el-upload__tip">
        文件需小于100MB以内
      </div>
    </template>
  </el-upload>
<p v-for="(item,i) in fileUrlList" :key="i">{{` ${i+1}: ${getUrl(item)}` }} <el-button></el-button></p>

    <template #footer>
      <span class="dialog-footer">
        <el-button @click="fileDialogVisible = false">取消</el-button>
        <el-button type="primary" @click="dialogVisible = false">
         确认
        </el-button>
      </span>
    </template>
  </el-dialog>
    </template>

  </mavon-editor>
</template>


<script setup>
import { mavonEditor } from 'mavon-editor'
import 'mavon-editor/dist/css/index.css'
import { ref, computed, watch, onMounted } from 'vue';
import { upload } from '@/apis/fileApi'
import EmojiPanel from '@/components/EmojiPanel.vue'

const md = ref(null);
const props = defineProps(['height', 'modelValue', "codeStyle"])
const emit = defineEmits(['update:modelValue'])
const fileDialogVisible=ref(false)

//已经上传好的文件列表url
const fileUrlList=ref([])

const fileUploadUrl=`${import.meta.env.VITE_APP_BASEAPI}/file`
// //v-model传值出去
const text = computed({
  get() {
    return props.modelValue
  },
  set(value) {
    emit('update:modelValue', value)
  }
})

const getUrl= (str)=>{
return `${import.meta.env.VITE_APP_BASEAPI}/file/${str}`
}

//关闭文件上传弹窗
const fileHandleClose=()=>{
fileDialogVisible.value=false;
}
//文件上传成功后
const onSuccess=(response)=>{
  fileUrlList.value.push(response.data[0].id)
 
}
//图片上传
const imgAdd = async (pos, $file) => {
  // 第一步.将图片上传到服务器.
  var formdata = new FormData();
  formdata.append('file', $file);
  const response = await upload(formdata)
  const url = `${import.meta.env.VITE_APP_BASEAPI}/file/${response.data[0].id}/true`;
  //console.log(url)
  md.value.$img2Url(pos, url);

}

//选择表情包
const onSelectEmoji=(emoji)=>{
  //console.log(emoji.i,"emoji");
  text.value+=emoji.i
}

const change=(value,render)=>{


}


</script>


<style  scoped>
.edit {

width: 100%;
}


</style>