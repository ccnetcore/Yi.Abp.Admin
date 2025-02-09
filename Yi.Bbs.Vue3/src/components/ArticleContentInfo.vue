<style scoped lang="scss">

//::v-deep(ol .li-list){
//  list-style: decimal;
//}
//::v-deep(ul .li-list){
//  list-style: disc;
//}



::v-deep(.pre-out)
{
padding: 0;
overflow-x: hidden;
}
::v-deep(.pre) {
    padding: 0;
    margin-bottom: 0;
    overflow-x: hidden;
    .header {
        background-color: #409eff;
        color: white;
        height: 30px;
        display: flex;
        justify-content: flex-end;
        padding-top: 10px;


        .language {}
        .copy:hover {
  cursor: pointer;
}
        .copy {
            margin: 0px 10px;

        }
    }

    .code-con {
        display: flex;

        .nav {
            display: block;
            background-color: #282C34;

        }

        .code {
            display: block;
            padding: 10px 10px;
            font-size: 14px;
            line-height: 22px;
            border-radius: 4px;
            overflow-x: auto;
        }

    }


}

::v-deep(.nav-ul) {
    border-right: 1px solid #FFFFFF;
    margin-top: 12px;
    padding-left: 10px;
    padding-right: 2px;

    .nav-li {
        margin: 5.3px 0;
        text-align: right;
        margin-right: 3px;
    }
}
</style>
<template>
    <div>
        <div class="markdown-body" v-html="outputHtml"></div>
    </div>
</template>
  
<script setup>
import { marked } from 'marked';
import '@/assets/atom-one-dark.css';
import '@/assets/github-markdown.css';
import hljs from "highlight.js";
//可以设置加载样式切换主题

import {nextTick , ref, watch } from 'vue';
const BREAK_LINE_REGEXP = /\r\n|\r|\n/g;


const outputHtml = ref("")
const props = defineProps(['code'])
let codeCopyDic=[];
const addCopyEvent=()=>{
    const copySpans = document.querySelectorAll('.copy');
// 为每个 copy span 元素添加点击事件
copySpans.forEach(span => {
    
  span.addEventListener('click', async function() {
   await navigator.clipboard.writeText(codeCopyDic.filter(x=>x.id==span.id)[0].code );
   ElMessage({
          message: "代码块复制成功",
          type: "success",
          duration: 2000,
        });
  });
});
}

//code部分处理、高亮
const codeHandler = (code, language) => {
    const codeIndex = parseInt(Date.now() + "") + Math.floor(Math.random() * 10000000);
    // 格式化第一行是右侧language和 “复制” 按钮；
    if (code) {
        const navCode = navHandler(code)
        try {
            // 使用 highlight.js 对代码进行高亮显示
            const preCode = hljs.highlightAuto(code).value;
            // 将代码包裹在 textarea 中，由于防止textarea渲染出现问题，这里将 "<" 用 "&lt;" 代替，不影响复制功能
            let html = `<pre  class='hljs pre'><div class="header"><span class="language">${language}</span><span class="copy" id="${codeIndex}">复制代码</span></div><div class="code-con"><div class="nav">${navCode}</div><code class="code">${preCode}</code></div></pre>`;
            codeCopyDic.push({id: codeIndex,code:code});
            // console.log(codeCopyDic.length);
            return html;
            //<textarea style="position: absolute;top: -9999px;left: -9999px;z-index: -9999;" id="copy${codeIndex}">${code.replace(/<\/textarea>/g, "&lt;/textarea>")}</textarea>
        } catch (error) {
            console.log(error);
        }
    }

}
//左侧导航栏处理
const navHandler = (code) => {
    //获取行数
    var linesCount = getLinesCount(code);

    var currentLine = 1;
    var liHtml = ``;
    while (linesCount + 1 >= currentLine) {
        liHtml += `<li class="nav-li">${currentLine}</li>`
        currentLine++
    }

    let html = `<ul class="nav-ul">${liHtml}</ul>`


    return html;
}

const getLinesCount = (text) => {
    return (text.trim().match(BREAK_LINE_REGEXP) || []).length;
}
watch(props, (n, o) => {
    codeCopyDic=[];
    marked.setOptions({
        renderer: new marked.Renderer(),
        highlight: function (code, language) {
            return codeHandler(code, language);
            // return hljs.highlightAuto(code).value;
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
   const soureHtml  = marked(n.code);
    outputHtml.value= soureHtml.replace(/<pre>/g, '<pre class="pre-out">').replace(/<li>/g, '<li class="li-list">');
        nextTick(()=>{
            addCopyEvent();
        }) 
}, { immediate: true, deep: true })
</script>

