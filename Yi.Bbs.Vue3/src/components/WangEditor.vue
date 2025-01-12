<template>
  <div style="border: 1px solid #ccc">
    <Toolbar
        style="border-bottom: 1px solid #ccc"
        :editor="editorRef"
        :defaultConfig="toolbarConfig"
        :mode="mode"
    />
    <Editor
        style="overflow-y: hidden;"
        :style="{height:props.height}"
        v-model="model"
        :defaultConfig="editorConfig"
        :mode="mode"
        @onCreated="handleCreated"
    />
  </div>
</template>
<script setup>
import '@wangeditor/editor/dist/css/style.css' // 引入 css

import { onBeforeUnmount, ref, shallowRef, onMounted } from 'vue'
import { Editor, Toolbar } from '@wangeditor/editor-for-vue'




const props = defineProps(["height"]);
// 编辑器实例，必须用 shallowRef
const editorRef = shallowRef()

const  mode= 'default';
const model = defineModel()
const toolbarConfig = {
  excludeKeys:[
    "uploadVideo","insertVideo","uploadImage","editVideoSize"
  ]
}
const editorConfig = { 
  placeholder: '发表一个友善的评论吧...',
  maxLength:2000
}

// 组件销毁时，也及时销毁编辑器
onBeforeUnmount(() => {
  const editor = editorRef.value
  if (editor == null) return
  editor.destroy()
})

const handleCreated = (editor) => {
  editorRef.value = editor // 记录 editor 实例，重要！
  console.log(editorRef.value.getAllMenuKeys(),"ss")
}
</script> 