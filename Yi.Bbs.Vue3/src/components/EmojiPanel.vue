<template>
  <EmojiPicker :native="true" @select="onSelectEmoji" />
</template>

<script setup>
import EmojiPicker from "vue3-emoji-picker";
import "vue3-emoji-picker/css";
const emits=defineEmits(['onSelect'])
const onSelectEmoji = (emoji) => {
    emits('onSelect',emoji)

  /*
    // result
    { 
        i: "😚", 
        n: ["kissing face"], 
        r: "1f61a", // with skin tone
        t: "neutral", // skin tone
        u: "1f61a" // without tone
    }
    */
};
</script>