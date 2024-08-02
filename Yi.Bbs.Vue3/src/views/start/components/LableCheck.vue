<script setup>
import { ref } from 'vue';
const model = defineModel()
const props = defineProps(["title", "text"]);


</script>
<template>
    <div class="body">
        <input class="check" type="checkbox" v-model="model" />
        <div class="right-box">
            <h4>{{ props.title }}</h4>
            <p>{{ props.text }}</p>
        </div>
    </div>
</template>
<style lang="scss" scoped>
.body {
    display: flex;
    margin-top: 30px;
}

.right-box {
    margin-left: 10px;
}

h4 {
    color: #292d33;
    font-size: 16px !important;
    font-weight: 500 !important;
    margin: 0 0;

}

p {
    margin-top: 5px;
    color: rgba(11, 22, 33, .6) !important;
    font-size: .875em;
}

input[type=checkbox] {
    cursor: pointer;
    position: relative;
    width: 25px;
    height: 25px;
    font-size: 12px;
}

input[type=checkbox]::after {
    position: absolute;
    top: 0;
    color: #000;
    width: 25px;
    height: 25px;
    display: inline-block;
    visibility: visible;
    padding-left: 0px;
    text-align: center;
    content: ' ';
    border-radius: 3px
}

input[type=checkbox]:checked::after {
    content: "✓";
    color: #fff;
    font-size: 16px;
    font-weight: bold;
    background-color: #409EFF;
}
</style>