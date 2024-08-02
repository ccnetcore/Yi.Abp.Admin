<script setup>
const model = defineModel()
const copyText=async ()=>{
    var oldText=model.value;
    var newText=oldText.replace(">", "").trim();
    await navigator.clipboard.writeText(newText);
    ElMessage({
          message: "命令复制成功",
          type: "success",
          duration: 2000,
        });
}
</script>
<template>
    <div class="box">
        <div class="header">
            <div class="header-left">

            <span class="dot"></span>
            <span class="dot"></span>
            <span class="dot"></span>
            </div>
            <div class="header-right">
<button @click="copyText">复制</button>
            </div>
        </div>
        <div class="body"> {{ model }}</div>
    </div>
</template>
<style scoped lang="scss">
.box {
    background-color: #292D33;

    width: 100%;
    border-radius: 12px;
    border: 4px solid #0c0e12;
    color: #fff;
    padding: 18px 18px 12px !important;
    font-size: 15px;
    line-height: 25px;
    margin: 15px 0;
}

.dot {
    display: flex;
    margin-right: 8px;
    height: 10px;
    width: 10px;
    border-radius: 50%;
}

.dot:nth-child(1) {
    background-color: #f24e1e;
}

.dot:nth-child(2) {
    background-color: #ffc700;
}

.dot:nth-child(3) {
    background-color: #0fa958;
}

.header {
    display: flex;

    height: 30px;
    &-left{
        display: flex;
    }
    &-right{
        button{
            background-color:#409EFF;
            color: #fff !important;
            height: 25px;
            width: 50px;
        }
    }
    justify-content: space-between;
}
</style>