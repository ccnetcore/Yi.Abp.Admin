<script setup>
import { computed, onMounted, onUnmounted, reactive, ref } from "vue";
import CodeBox from "./components/CodeBox.vue"
import LableInput from "./components/LableInput.vue"
import SlectBox from "./components/SlectBox.vue"
import LableCheck from "./components/LableCheck.vue"
import {GetResult} from '@/apis/nugetApi'


const isFixed = ref(false);
const form = reactive({
    name: "Acme.BookStore",
    isCsf: true,
    dbType: 'sqlite',
    type:"module"
});
const installText = "> dotnet tool install -g Yi.Abp.Tool";
const cloneText = "> yi-abp clone ";
const listText="> yi-abp new list ";
const helpText="> yi-abp -h ";
const nugetData=reactive({
    versions:"0.0.0",
    downloadNumber:0

});

const onDbTypeSelected = (data) => {
    form.dbType = data.value;
}
const onProjectSelected=(data)=>{

}
const dbData = [
    { name: 'Sqlite', key: 'sqlite', value: 'sqlite' },
    { name: 'Mysql', key: 'mysql', value: 'mysql' },
    { name: 'SqlServer', key: 'sqlserver', value: 'sqlserver' },
    { name: 'Oracle', key: 'oracle', value: 'oracle' },
    { name: 'PostgreSql', key: 'postgresql', value: 'postgresql' },

];


const typeData = [{ name: '模块', key: 'module', value: 'module' },
{ name: '模块', key: 'module', value: 'module' },
  { name: '模块', key: 'module', value: 'module' },
  { name: '模块', key: 'module', value: 'module' }]
const addModuleComputed=computed(()=>{

    return `> yi-abp add-module ${form.name}`;
})

const commandComputed=computed(()=>{
let dbType=form.dbType;
if(dbType=="sqlite")
{
    dbType=""
}


return `> yi-abp new ${form.name} -t module ${dbType!=''?'-dbms '+form.dbType:''} ${form.isCsf==true?'-csf':''}`
});

onMounted(async() => {
    const {data}= await GetResult();
    nugetData.downloadNumber=data.downloadNumber;
    nugetData.versions=data.versions[0];
    //console.log(data,"data");
    // 监听页面滚动事件
    window.addEventListener("scroll", scrolling, true);

})
const scrolling = () => {

    // 滚动条距文档顶部的距离
    let scrollTop = document.getElementById('main-box').scrollTop;
    // 滚动条滚动的距离
    let commandBoxTop = document.getElementById('command').offsetTop;

    var width = document.getElementById('command').getBoundingClientRect().width;

    document.getElementById('command').style.width = width + 'px';
    if (scrollTop > commandBoxTop) {

        isFixed.value = true;
    }
    else {
        isFixed.value = false;
    }

}


onUnmounted(() => {
    window.removeEventListener("scroll", scrolling, true);
})

</script>
<template>
    <div class="start-body">

        <div class="content">
            <div class="content-title"><span>开始</span>
            
            <div class="version">Yi.Abp.Tool工具集，最新版本号 {{ nugetData.versions }}，当前总下载次数 {{nugetData.downloadNumber}}</div>
            </div>
            <div class="content-body">
                <div class="content-body-left">
                    <h4>安装 Yi.Abp.Tool</h4>
                    <p>如果之前未安装 Yi.Abp.Tool，请在命令行终端中安装：</p>
                    <CodeBox  v-model="installText" />

                    <h4>克隆源代码，yi框架非打包，便于大家调试二开</h4>
                    <p>需安装git及Yi.Abp.Tool，执行命令：</p>
                    <CodeBox  v-model="cloneText" />


                    <h4>创建你的模块</h4>
                    <p>在module文件夹内，命令行终端运行以下命令：</p>
                    <CodeBox id="command" :class="{ command: isFixed }" v-model="commandComputed" />


                    <h4>将你创建的模块添加到当前解决方案中</h4>
                    <p>在module文件夹内，命令行终端运行以下命令：</p>
                    <CodeBox  v-model="addModuleComputed" />
                    <p>模块创建后，可选择任意host主机进行模块依赖即可</p>

                    <h4>配置</h4>
                    <p>您可以更改下面的解决方案配置。</p>


                    <h5>为项目命名</h5>
                    <LableInput v-model="form.name" />
                    <p>您可以使用不同级别的命名空间；例如，BookStore、Acme.BookStore 或 Acme.Retail.BookStore。</p>


                    <h5>选择创建类型</h5>
                    <SlectBox :data="typeData" width="25%" @onSelected="onProjectSelected" />


                    <h5>选择数据库管理系统</h5>
                    <SlectBox :data="dbData" width="20%" @onSelected="onDbTypeSelected" />


                    <LableCheck v-model="form.isCsf" title="创建解决方案文件夹" text="指定项目是放在输出文件夹中的新文件夹中，还是直接放在输出文件夹中。" />
                   <p>默认勾选</p>

                  <h4>模板库</h4>
                  <p>现在已经支持公开模板库，地址：https://gitee.com/ccnetcore/yi-template</p>
                  <p>每个分支代表一个模板，你可以通过提交pr，或联系管理员上传你自定义的模板</p>
                  <CodeBox  v-model="listText" />
                  <p>通过以上命令，可查询当前可用模板</p>


                  <h4>帮助</h4>
                  <p>更详细的实时帮助，可执行帮助命令</p>
                  <CodeBox  v-model="helpText" />
                  <p>如还有疑惑，或错误，可在社区对应板块发布问题</p>
                </div>
                <div class="content-body-right">

                </div>


            </div>
        </div>

    </div>
</template>
<style lang="scss" scoped>
.command {
    position: fixed !important;
    z-index: 99;
    top: 100px;
   width: 1000px;
}

.start-body {
    height: 100%;
    width: 100%;
    background-color: #FCFCFC;


}

.content {
    width: 80%;
    margin: 0 auto 50px auto;

    &-title {
        background-color: #FCFCFC;
        height: 100px;
        display: flex;
        align-items: center;

        .version{
            height: 100%;
    display: flex;
    align-items: flex-end;
    padding: 30px 10px;
    color: darkgrey;
        }
        span {
            color: #292d33;
            font-size: 48px;
            font-weight: 700;
        }
    }

    &-body {
        height: 1800px;
        padding: 48px;
        background-color: #fff;
        border-radius: 12px;
        border: 0;
        box-shadow: 0 0 1rem rgba(0, 0, 0, .08);
        display: flex;

        &-left {
            width: 100%;
        }

        &-right {
            width: 0%;
            background-color: #409EFF;
        }



        p {
            margin-top: 0;
            margin-bottom: 1rem;
            font-family: "Poppins";
            font-size: 14px;
            font-weight: 300;
            color: rgba(11, 22, 33, .6) !important;
        }

        h4 {
            font-size: 20px;
            font-weight: 500;
            color: #409EFF;
            margin-bottom: .25rem;
            margin-top: 0;

        }

        h5 {
            color: #292d33;
            font-size: 16px !important;
            font-weight: 500 !important;
            margin: 20px 0 10px 0;
        }
    }
}
</style>