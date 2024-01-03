<template>
    <div>
        <div class="markdown-body" v-html="outputHtml"></div>
    </div>
</template>
  
<script setup>
import { marked } from 'marked';

import hljs from "highlight.js";
//可以设置加载样式切换主题
import 'highlight.js/styles/atom-one-dark.css'
import '@/assets/github-markdown.css'
import { ref,watch  } from 'vue';



const outputHtml=ref("")
const props = defineProps(['code'])
watch(props,(n,o)=>{
    marked.setOptions({
        renderer: new marked.Renderer(),
        highlight: function(code) {
            return hljs.highlightAuto(code).value;
          },
        pedantic: false,
        gfm: true,//允许 Git Hub标准的markdown
        tables: true,//支持表格
    
        breaks: true,
        sanitize: false,
        smartLists: true,
        smartypants: false,
        xhtml: false,
        smartLists: true,
    }
    );
    //需要注意代码块样式
    outputHtml.value = marked(n.code).replace(/<pre>/g, "<pre class='hljs'>")
})

</script>

