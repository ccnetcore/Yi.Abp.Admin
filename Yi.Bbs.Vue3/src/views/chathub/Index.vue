<script setup>
import {nextTick, onMounted, ref, computed, onUnmounted} from 'vue';
import {storeToRefs} from 'pinia'
import useAuths from '@/hooks/useAuths.js';
import {getList as getChatUserList} from '@/apis/chatUserApi'
import {
  sendPersonalMessage,
  sendGroupMessage,
  getAccountList as getChatAccountMessageList,
  sendAiChat
} from '@/apis/chatMessageApi'
import useChatStore from "@/stores/chat";
import useUserStore from "@/stores/user";

const {isLogin} = useAuths();
import {useRouter} from 'vue-router'
import {getUrl} from '@/utils/icon'

//markdown ai显示
import {marked} from 'marked';
import markedKatex from "marked-katex-extension";
import '@/assets/atom-one-dark.css';
import '@/assets/github-markdown.css';
import hljs from "highlight.js";

const isShowTipNumber = ref(10);
const router = useRouter();
//聊天存储
const chatStore = useChatStore();
const {userList} = storeToRefs(chatStore);

//用户信息
const userStore = useUserStore();
//发送消息是否为空
const msgIsNullShow = ref(false)
//当前选择用户
const currentSelectUser = ref('all');
//当前输入框的值
const currentInputValue = ref("");
//临时存储的输入框，根据用户id及组name、all组为key，data为value
const inputListDataStore = ref([
  {key: "all", name: "官方学习交流群", titleName: "官方学习交流群", logo: "yilogo.png", value: ""},

  {key: "ai@gpt-4o-mini", name: "ChatGpt聊天", titleName: "ChatGpt-全能神！综合能力最强！", logo: "openAi.png", value: ""},

  {key: "ai@claude-3-7-sonnet", name: "Claude聊天", titleName: "Claude3.7 代码逻辑地表最强！", logo: "claudeAi.png", value: ""},
  {key: "ai@claude-3.7-sonnet-thinking", name: "Claude思索", titleName: "Claude3.7 思索模式，强中强！", logo: "claudeAi.png", value: ""},

  {key: "ai@grok-3", name: "Grok聊天", titleName: "Grok3 为3.0王的诞生献上礼炮", logo: "grokAi.png", value: ""},

  {key: "ai@Qwen/QwQ-32B-Preview", name: "QWen聊天", titleName: "国产阿里千问通义", logo: "qwenAi.png", value: ""},

  {key: "ai@DeepSeek-V3", name: "DeepSeek聊天", titleName: "满血DeepSeek-聊天模式，开源模型第一", logo: "deepSeekAi.png", value: ""},
  {key: "ai@DeepSeek-R1", name: "DeepSeek思索", titleName: "满血DeepSeek-思索模式", logo: "deepSeekAi.png", value: ""}
]);
//AI聊天临时存储
const sendAiChatContext = ref([]);
var timer = null;

let codeCopyDic = [];

// 添加可调整大小的变量
const middleWidth = ref(380);
const contentHeight = ref(535);
const isDraggingVertical = ref(false);
const isDraggingHorizontal = ref(false);
const startX = ref(0);
const startY = ref(0);

//初始化
onMounted(async () => {
  if (!isLogin.value) {
    ElMessage({
      message: '该功能，请登录后使用！即将自动跳转',
      type: 'warning',
    })
    timer = setTimeout(function () {
      onclickClose();
    }, 3000);
  }
  marked.use(markedKatex({
    throwOnError: false,
    nonStandard: true
  }));
  marked.setOptions({
        renderer: new marked.Renderer(),
        highlight: function (code, language) {
          return codeHandler(code, language);
        },
        pedantic: false,
        gfm: true,//允许 Git Hub标准的markdown
        tables: true,//支持表格
        breaks: true,
        sanitize: false,
        smartypants: false,
        xhtml: false,
        smartLists: true,
      }
  );
  //all的聊天消息
  chatStore.setMsgList((await getChatAccountMessageList()).data);
  //在线用户列表
  chatStore.setUserList((await getChatUserList()).data);
  startCountTip();

  // 添加全局鼠标事件监听
  document.addEventListener('mousemove', handleMouseMove);
  document.addEventListener('mouseup', handleMouseUp);
})

onUnmounted(() => {
  if (timer != null) {
    clearInterval(timer)
  }
  if (timerTip != null) {
    clearInterval(timerTip)
  }
  
  // 移除全局鼠标事件监听
  document.removeEventListener('mousemove', handleMouseMove);
  document.removeEventListener('mouseup', handleMouseUp);
})

// 开始垂直拖拽（左右分割线）
const startDragVertical = (e) => {
  isDraggingVertical.value = true;
  startX.value = e.clientX;
  e.preventDefault();
};

// 开始水平拖拽（上下分割线）
const startDragHorizontal = (e) => {
  isDraggingHorizontal.value = true;
  startY.value = e.clientY;
  e.preventDefault();
};

// 处理鼠标移动事件
const handleMouseMove = (e) => {
  if (isDraggingVertical.value) {
    const deltaX = e.clientX - startX.value;
    const newWidth = middleWidth.value + deltaX;
    
    // 限制最小宽度和最大宽度
    if (newWidth >= 250 && newWidth <= 500) {
      middleWidth.value = newWidth;
      startX.value = e.clientX;
    }
  }
  
  if (isDraggingHorizontal.value) {
    const deltaY = e.clientY - startY.value;
    const newHeight = contentHeight.value + deltaY;
    
    // 限制最小高度和最大高度
    if (newHeight >= 300 && newHeight <= 600) {
      contentHeight.value = newHeight;
      startY.value = e.clientY;
    }
  }
};

// 处理鼠标松开事件
const handleMouseUp = () => {
  isDraggingVertical.value = false;
  isDraggingHorizontal.value = false;
};

/*-----计算属性-----*/
//当前聊天框内容显示的消息
const currentMsgContext = computed(() => {
  //选择全部人
  if (selectIsAll()) {
    return chatStore.allMsgContext;
  }
  //选择ai
  else if (selectIsAi()) {
    //如果是ai的值，还行经过markdown处理
    let tempHtml = [];
    codeCopyDic = [];
    chatStore.getMsgContextFunc(currentSelectUser.value).forEach(element => {
      tempHtml.push({
        content: toMarkDownHtml(element.content),
        messageType: 'Ai',
        sendUserId: element.sendUserId,
        sendUserInfo: element.sendUserInfo
      });
    });

    return tempHtml;
  }
  //选择个人
  else {
    return chatStore.personalMsgContext.filter(x => {
      //两个条件
      //接收用户者id为对面id（我发给他）
      //或者，发送用户id为对面（他发给我）
      return (x.receiveId === currentSelectUser.value.userId && x.sendUserId === userStore.id) ||
          (x.sendUserId === currentSelectUser.value.userId && x.receiveId === userStore.id);
    });
  }
});
//获取聊天内容的头像
const getChatUrl = (url, position) => {
  if (position === "left" && selectIsAi()) {
    return imageSrc(inputListDataStore.value.find(x => x.key === currentSelectUser.value).logo)
  }
  return getUrl(url);
}
//当前聊天框显示的名称
const currentHeaderName = computed(() => {
  if (selectIsAll() || selectIsAi()) {
    return inputListDataStore.value.find(x => x.key === currentSelectUser.value).titleName;
  } else {
    return currentSelectUser.value.userName;
  }
});
//当前在线的用户项
const currentUserItem = computed(() => {
  return userList.value.filter(x => x.userId !== useUserStore().id)
});

/*-----方法-----*/
//当前选择的是否为全部
const selectIsAll = () => {
  return currentSelectUser.value === 'all';
};
//当前选择的是否为Ai
const selectIsAi = () => {
  //以ai@开头
  return /^ai@/.test(currentSelectUser.value);
};
//是否为公共的类型
const isPublicType = (itemType) => {
  return itemType === 'all' || /^ai@/.test(itemType);
}

//输入框的值被更改（同时保存到store中）
const changeInputValue = (inputValue) => {
  currentInputValue.value = inputValue;
  let index = -1;
  let findKey = currentSelectUser.value?.userId
  if (selectIsAll()) {
    findKey = 'all';
  } else if (selectIsAi()) {
    findKey = currentSelectUser.value;
  }
  index = inputListDataStore.value.findIndex(obj => obj.key === findKey);
  inputListDataStore.value[index].value = currentInputValue.value;
}

//获取输入框的值（切换左侧菜单的时候，给输入框补充之前没发送的值）
const getCurrentInputValue = () => {
  if (selectIsAll()) {
    return inputListDataStore.value.filter(x => x.key === "all")[0].value;
  } else if (selectIsAi()) {
    return inputListDataStore.value.filter(x => x.key === currentSelectUser.value)[0].value;
  } else {
    //如果不存在初始存储值
    if (!inputListDataStore.value.some(x => x.key === currentSelectUser.value.userId)) {
      inputListDataStore.value.push({key: currentSelectUser.value.userId, value: "", type: 'user'});
      return "";
    }
    return inputListDataStore.value.filter(x => x.key === currentSelectUser.value.userId)[0].value;
  }
};

//点击用户列表,
const onclickUserItem = (userInfo, itemType) => {
  //公共
  if (isPublicType(itemType)) {
    currentSelectUser.value = itemType;
  }
  //个人
  else {
    currentSelectUser.value = userInfo;
  }
  //填充临时存储的输入框
  var value = getCurrentInputValue();
  //更新当前的输入框
  changeInputValue(value);
}

//点击发送按钮
const onclickSendMsg = () => {
  //发送空消息
  if (currentInputValue.value === "") {
    msgIsNullShow.value = true;
    setTimeout(() => {
      // 这里写上你想要3秒后执行的代码
      msgIsNullShow.value = false;
    }, 3000);
    return;
  }

  if (selectIsAll()) {
    onclickSendGroupMsg("all", currentInputValue.value);
  } else if (selectIsAi()) {
    //ai消息需要将上下文存储
    sendAiChatContext.value.push({
      answererType: 'User',
      message: currentInputValue.value,
      number: sendAiChatContext.value.length,
      messageType: currentSelectUser.value
    })

    //离线前端存储
    chatStore.addMsg({
      messageType: currentSelectUser.value,
      content: currentInputValue.value,
      sendUserId: userStore.id,
      sendUserInfo: {user: {icon: userStore.icon}}
    })
    //ai模型，去掉key的ai@开头的字符串
    const model = currentSelectUser.value.replace(/^ai@/, '');
    //上下文内容，当前ai进行隔离
    const content = sendAiChatContext.value.filter(x => x.messageType === currentSelectUser.value)

    //发送ai消息
    sendAiChat(content, model);
  } else {
    onclickSendPersonalMsg(currentSelectUser.value.userId, currentInputValue.value);
  }
  changeInputValue("");
}

//点击发送个人消息
const onclickSendPersonalMsg = (receiveId, msg) => {
  //添加到本地存储
  chatStore.addMsg({
    messageType: "Personal",
    sendUserId: userStore.id,
    content: msg,
    receiveId: receiveId
  });
  sendPersonalMessage({userId: receiveId, content: msg});
  //调用接口发送消息
}

//点击发送群组消息按钮
const onclickSendGroupMsg = (groupName, msg) => {
  //组还需区分是否给全部成员组
  if (selectIsAll) {
    sendGroupMessage({content: msg});
  } else {
    alert("暂未实现");
  }
}

//获取当前最后一条信息（显示在左侧菜单）
const getLastMessage = ((receiveId, itemType) => {
  if (isPublicType(itemType)) {
    const message = chatStore.getMsgContextFunc(itemType);
    return message[message.length - 1]?.content.substring(0, 15);
  } else {
    const messageContext = chatStore.personalMsgContext.filter(x => {
      //两个条件
      //接收用户者id为对面id（我发给他）
      //或者，发送用户id为对面（他发给我）
      return (x.receiveId === receiveId && x.sendUserId === userStore.id) ||
          (x.sendUserId === receiveId && x.receiveId === userStore.id);
    });
    return messageContext[messageContext.length - 1]?.content.substring(0, 15);
  }

})


/*----------以下方法是对内容通用处理，没有业务逻辑，无需变化----------*/
const imageSrc = (imageName) => {
  // 动态拼接路径
  return new URL(`../../assets/chat_images/${imageName}`, import.meta.url).href;
}

//绑定的input改变事件
const updateInputValue = (event) => {
  changeInputValue(event.target.value);
}

//输入框按键事件
const handleKeydownInput = () => {
  // 检查是否按下 Shift + Enter
  if (event.key === 'Enter' && event.shiftKey) {
    // 允许输入换行
    return; // 让默认行为继续
  }
  // 如果只按下 Enter，则阻止默认的提交行为，比如在表单中
  if (event.key === 'Enter') {
    // 阻止默认行为
    event.preventDefault();
    onclickSendMsg();
    return;
  }
}

//关闭聊天框
const onclickClose = () => {
  router.push({path: "/index"})
      .then(() => {
        // 重新刷新页面
        location.reload()
      })
}

//清除ai对话
const clearAiMsg = () => {
  sendAiChatContext.value = [];
  chatStore.clearTypeMsg(currentSelectUser.value);
  ElMessage({
    message: "当前Ai会话清除成功",
    type: "success",
    duration: 2000,
  });

}


//转换markdown
const toMarkDownHtml = (text) => {
  //处理数学公式
  let soureMd = text.replace(/\\\[/g, '$').replace(/\\\]/g, '$');
  //需要注意代码块样式
  let soureHtml = marked(soureMd);
  nextTick(() => {
    addCopyEvent();
  })
  return soureHtml;
}
//code部分处理、高亮
const codeHandler = (code, language) => {
  const codeIndex = parseInt(Date.now() + "") + Math.floor(Math.random() * 10000000);
  //console.log(codeIndex,"codeIndex");
  // 格式化第一行是右侧language和 "复制" 按钮；
  if (code) {
    const navCode = navHandler(code)
    try {
      // 使用 highlight.js 对代码进行高亮显示
      const preCode = hljs.highlightAuto(code).value;
      // 将代码包裹在 textarea 中，由于防止textarea渲染出现问题，这里将 "<" 用 "&lt;" 代替，不影响复制功能
      let html = `<pre  class='hljs pre'><div class="header"><span class="language">${language}</span><span class="copy" id="${codeIndex}">复制代码</span></div><div class="code-con"><div class="nav">${navCode}</div><code class="code">${preCode}</code></div></pre>`;
      codeCopyDic.push({id: codeIndex, code: code});
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
const BREAK_LINE_REGEXP = /\r\n|\r|\n/g;
const getLinesCount = (text) => {
  return (text.trim().match(BREAK_LINE_REGEXP) || []).length;
}

let timerTip = null;
//倒计时显示tip
const startCountTip = () => {
  timerTip = setInterval(() => {
    if (isShowTipNumber.value > 0) {
      isShowTipNumber.value--;
    } else {
      clearInterval(timerTip); // 倒计时结束
    }
  }, 1000);
};
//代码copy事件
const addCopyEvent = () => {
  const copySpans = document.querySelectorAll('.copy');
// 为每个 copy span 元素添加点击事件
  copySpans.forEach(span => {
    //先移除，再新增
    span.removeEventListener('click', clickCopyEvent);
    span.addEventListener('click', clickCopyEvent);
  });
}
const clickCopyEvent = async function (event) {
  const spanId = event.target.id;
  console.log(codeCopyDic, "codeCopyDic")
  console.log(spanId, "spanId")
  await navigator.clipboard.writeText(codeCopyDic.filter(x => x.id == spanId)[0].code);
  ElMessage({
    message: "代码块复制成功",
    type: "success",
    duration: 2000,
  });
}
</script>

<template>

  <div style="position: absolute; top: 0;left: 0;" v-show="isShowTipNumber>0">
    <p>当前版本：2.4.0</p>
    <p>tip:官方学习交流群每次发送消息消耗 1 钱钱</p>
    <p>tip:点击聊天窗口右上角"X"可退出</p>
    <p>tip:多人同时在聊天室时，左侧可显示其他成员</p>

    <p>tip:当前支持多种AI模式，由于接口收费原因，还请各位手下留情</p>
    <p>tip:ai对话为持续对话，已优化输出速度</p>
    <p>tip:ai对话只有本地存储了记录，可点击清除或刷新</p>
    <p>即将自动隐藏tip：{{ isShowTipNumber }}</p>
  </div>
  <div class="body">
    <div class="left">
      <div class="icon">

        <img :src="userStore.icon">
      </div>
      <ul class="top-icon">
        <li><img src="@/assets/chat_images/wechat.png"/></li>
        <li><img src="@/assets/chat_images/addressBook.png"/></li>
        <li><img src="@/assets/chat_images/collection.png"/></li>
        <li><img src="@/assets/chat_images/file.png"/></li>
        <li><img src="@/assets/chat_images/friend.png"/></li>
        <li><img src="@/assets/chat_images/line.png"/></li>
        <li><img src="@/assets/chat_images/look.png"/></li>
        <li><img src="@/assets/chat_images/sou.png"/></li>
      </ul>
      <ul class="bottom-icon">
        <li><img src="@/assets/chat_images/mini.png"/></li>
        <li><img src="@/assets/chat_images/phone.png"/></li>
        <li><img src="@/assets/chat_images/other.png"/></li>
      </ul>
    </div>

    <div class="middle" :style="{ width: middleWidth + 'px' }">
      <div class="header">
        <div class="header-div">
          <div class="search">
            <img src="@/assets/chat_images/search.png"/>
            <span>搜索</span>
          </div>
          <button type="button"><img src="@/assets/chat_images/add.png"/></button>
        </div>
      </div>
      <div class="user-list">
        <div v-for="item in inputListDataStore.filter(x=>x.type!=='user')" :key="item.key" class="user-div"
             @click="onclickUserItem(null, item.key)"
             :class="{ 'select-user-item': currentSelectUser === item.key }">
          <div class="user-div-left">
            <img style="height: 48px;width: 48px;  " :src="imageSrc(item.logo)"/>
            <div class="user-name-msg">
              <p class="font-name">{{ item.name }}</p>
              <p class="font-msg">{{ getLastMessage(null, item.key) }}</p>
            </div>
          </div>
          <div class=" user-div-right">
            10:28
          </div>
        </div>

        <div v-for="(item, i) in currentUserItem" :key="i" @click="onclickUserItem(item, 'user')" class="user-div"
             :class="{ 'select-user-item': currentSelectUser?.userId === item.userId }">
          <div class="user-div-left">
            <img :src="getChatUrl(item.userIcon)"/>
            <div class="user-name-msg">
              <p class="font-name">{{ item.userName }}</p>
              <p class="font-msg">{{ getLastMessage(item.userId, 'user') }}</p>
            </div>
          </div>
          <div class=" user-div-right">
            10:28
          </div>
        </div>
      </div>
    </div>

    <!-- 垂直分割线 -->
    <div class="vertical-resizer" @mousedown="startDragVertical"></div>

    <div class="right" :style="{ width: 'calc(1400px - ' + (middleWidth + 70) + 'px)' }">
      <div class="header">
        <div class="header-left">{{ currentHeaderName }} <span class="clear-msg" v-show="selectIsAi()"
                                                               @click="clearAiMsg">点击此处清空当前对话</span>
        </div>

        <div class="header-right">
          <div>
            <ul>
              <li><img src="@/assets/chat_images/fixed.png"/></li>
              <li><img src="@/assets/chat_images/min.png"/></li>
              <li><img src="@/assets/chat_images/max.png"/></li>
              <li style="cursor: pointer;" @click="onclickClose"><img src="@/assets/chat_images/close.png"/></li>
            </ul>
          </div>
          <div class="more"><img src="@/assets/chat_images/other2.png"/></div>


        </div>

      </div>
      <div class="content" :style="{ height: contentHeight + 'px' }">
        <div v-for="(item, i) in currentMsgContext" :key="i">


          <!-- 对话框右侧 -->
          <div class="content-myself content-common" v-if="item.sendUserId === userStore.id">
            <div class="content-myself-msg content-msg-common " v-html="item.content"></div>

            <img :src="getChatUrl(item.sendUserInfo?.user.icon, 'right')"/>
          </div>

          <!-- 对话框左侧 -->
          <div class="content-others content-common" v-else>
            <img :src="getChatUrl(item.sendUserInfo?.user.icon, 'left')"/>
            <div>

              <p v-if="selectIsAll()" class="content-others-username">{{ item.sendUserInfo?.user.userName }}</p>
              <div class="content-others-msg content-msg-common " :class="{ 'content-others-msg-group': selectIsAll() }"
                   v-html="item.content">

              </div>
            </div>
          </div>

        </div>

      </div>
      
      <!-- 水平分割线 -->
      <div class="horizontal-resizer" @mousedown="startDragHorizontal"></div>
      
      <div class="bottom" :style="{ height: 'calc(100% - ' + (contentHeight + 75) + 'px)' }">
        <div class="bottom-tool">

          <ul class="ul-left">
            <li><img src="@/assets/chat_images/emoji.png"/></li>
            <li><img src="@/assets/chat_images/sendFile.png"/></li>
            <li><img src="@/assets/chat_images/screenshot.png"/></li>
            <li><img src="@/assets/chat_images/chatHistory.png"/></li>
          </ul>

          <ul class="ul-right">
            <li><img src="@/assets/chat_images/landline.png"/></li>
            <li><img src="@/assets/chat_images/videoChat.png"/></li>
          </ul>

        </div>
        <!-- <div class="bottom-input" contenteditable="true" @input="updateInputValue"> -->
        <!-- <div class="bottom-input" contenteditable="true"  @input="updateInputValue">

        </div> -->
        <textarea class="bottom-input" v-model="currentInputValue" @input="updateInputValue"
                  @keydown="handleKeydownInput"

        >

</textarea>
        <div class="bottom-send">
          <div class="msg-null" v-show="msgIsNullShow">不能发送空白信息</div>
          <button @click="onclickSendMsg()">
            发送(S)
          </button>

        </div>
      </div>
    </div>
  </div>
</template>
<style scoped lang="scss">
ul {
  list-style-type: none;
}

.body {
  height: 790px;
  width: 1400px;
  display: flex;
  justify-content: flex-start;
  box-shadow: 0px 0px 5px rgba(0, 0, 0, 0.2);
  position: relative;
}

/* 垂直分割线样式 */
.vertical-resizer {
  width: 5px;
  height: 100%;
  background-color: #e7e7e7;
  cursor: col-resize;
  z-index: 10;
}

.vertical-resizer:hover {
  background-color: #c0c0c0;
}

/* 水平分割线样式 */
.horizontal-resizer {
  width: 100%;
  height: 5px;
  background-color: #e7e7e7;
  cursor: row-resize;
  z-index: 10;
}

.horizontal-resizer:hover {
  background-color: #c0c0c0;
}

.select-user-item {
  background-color: #C8C8CA !important;
}

.left {
  background-color: #2a2a2a;
  width: 70px;
  padding: 46px 10px 0 10px;
  flex-shrink: 0;

  .icon {
    background-color: burlywood;
    height: 46px;
    width: 46px;

    img {
      height: 100%;
      width: 100%;
    }
  }
}

.middle {
  background-color: #dadbdc;
  width: 380px;
  flex-shrink: 0;

  .header {
    height: 75px;
    background: #f7f7f7;
    padding: 26px 0 26px 0;

    .header-div {
      background-color: #F7F7F7;
      height: 30px;
      width: 90%;
      margin: auto;
      display: flex;
      justify-content: space-between;

      .search {
        width: 85%;
        height: 100%;
        background-color: #E2E2E2;
        border-radius: 5px;
        padding: 5px;

        img {
          height: 16px;
          width: 16px;
        }

        span {
          margin-left: 10px;
          font-size: 14px;
          color: #818181;
          margin-bottom: 5px;
          vertical-align: top;
        }
      }

      button {
        margin-left: 10px;
        width: 30px;
        background-color: #E2E2E2;
        border-radius: 5px;
        padding: 5px 3px;

        img {
          height: 100%;
          width: 100%;
        }
      }
    }

  }

  .user-list::-webkit-scrollbar-thumb {
    background-color: #BEBCBA;
    /* 滚动条滑块颜色 */
  }

  .user-list::-webkit-scrollbar {
    width: 8px;
    /* 滚动条宽度 */
    height: 10px;
    /* 滚动条高度 */
  }

  .user-list {
    height: calc(100% - 75px);


    overflow-y: auto;

    /* 只启用垂直方向滚动条 */
    .user-div {
      display: flex;
      justify-content: space-between;
      height: 80px;
      width: 100%;
      background-color: #EAE8E7;
      padding: 16px;

      &-left {

        height: 100%;
        display: flex;

        .user-name-msg {
          display: flex;
          flex-direction: column;
          justify-content: space-between;
          padding: 0 10px;

          .font-name {
            font-size: 20px;
          }

          .font-msg {
            font-size: 15px;
            color: #999999;
          }
        }

        img {
          height: 100%;

        }
      }

      &-right {
        font-size: 15px;
        color: #999999;
      }
    }

    .user-div:hover {
      background-color: #D9D8D8;
    }
  }
}

.right {
  background-color: #f5f5f5;
  flex-grow: 1;
  display: flex;
  flex-direction: column;

  .header {
    height: 75px;
    background: #f7f7f7;
    border: 1px solid #e7e7e7;
    display: flex;
    justify-content: space-between;
    flex-shrink: 0;

    .header-left {
      padding: 25px;
      font-size: 25px;
      align-content: center;
    }

    .header-right {
      ul {
        display: flex;

        li {
          padding: 8px 12.5px 10px 12.5px;

          img {
            height: 15px;
            width: 15px;
          }
        }
      }

      .more {
        padding: 0px 12.5px;
        display: flex;
        justify-content: flex-end;

        img {
          height: 18px;
          width: 18px;
        }
      }
    }
  }

  .content {
    overflow-y: auto;
    /* 只启用垂直方向滚动条 */
    padding: 20px 40px;
    flex-grow: 1;

  }

  .bottom {
    background: #f7f7f7;
    border-top: 1.5px solid #e7e7e7;
    padding: 15px 35px;
    display: flex;
    flex-direction: column;
    min-height: 150px;

    &-tool {
      display: flex;
      justify-content: space-between;
      height: 22px;
      margin-bottom: 5px;

      .ul-left {
        display: flex;

        li {
          width: 22px;
          height: 22px;
          margin-right: 20px;
        }

        img {
          width: 100%;
          height: 100%;
        }
      }

      .ul-right {
        display: flex;

        li {
          width: 22px;
          height: 22px;
          margin-right: 20px;
        }

        img {
          width: 100%;
          height: 100%;
        }
      }

    }

    &-input {
      font-family: "Microsoft YaHei", sans-serif;
      flex-grow: 1;
      width: 100%;
      overflow-y: auto;
      padding: 10px 0;
      font-size: 18px;
      background: #F7F7F7;
      border: none;
      resize: none;
      outline: none;
      min-height: 50px;
    }

    &-send {
      display: flex;
      justify-content: flex-end;

      button {
        width: 126px;
        height: 40px;
        background-color: #E9E9E9;
        color: #06AE56;
        border-radius: 5px;
        font-size: 18px;
      }

      button:hover {
        background-color: #D2D2D2;

      }
    }
  }
}

.top-icon {
  margin-top: 32px;

  li {
    cursor: pointer;
    text-align: center;
    margin-bottom: 24px;

    img {
      height: 24px;
      width: 24px;

    }

  }
}

.bottom-icon {
  margin-top: 125px;

  li {
    cursor: pointer;
    text-align: center;
    margin-bottom: 24px;

    img {
      height: 24px;
      width: 24px;

    }

  }
}


.content-common {
  display: flex;
  margin-bottom: 18px;

  img {
    height: 45px;
    width: 45px;
  }
}

.content-msg-common {
  // display: flex;
  align-content: center;
  flex-wrap: wrap;
  position: relative;

  margin: 0 15px;
  padding: 5px 15px;
  text-align: center;
  font-size: 18px;
  border-radius: 5px;
  max-width: 600px;
  text-align: justify;
}


.content-myself {
  justify-content: flex-end;
}

.content-myself-msg:hover {

  background-color: #89D961;
}

.content-myself-msg {
  background-color: #95EC69;
}

.content-myself-msg:hover:after {
  border-left: 10px solid #89D961;
}


.content-others {
  justify-content: flex-start;

}

.content-others-username {
  margin-left: 15px;
  color: #B2B2B2;
  position: relative;
  top: -15px;
}

.content-others-msg-group {
  position: relative;
  top: -10px;
}

.content-others-msg {
  background-color: #FFFFFF;
  padding: 10px 15px;

}

.content-others-msg:hover {
  background-color: #EBEBEB;
}

.content-others-msg:hover:after {
  border-right: 10px solid #EBEBEB;
}

.content-myself-msg:after {
  content: '';
  position: absolute;
  width: 0;
  height: 0;
  /* 箭头靠右边 */
  top: 13px;
  right: -10px;
  border-top: 10px solid transparent;
  border-bottom: 10px solid transparent;
  border-left: 10px solid #97EC6F;

}

.content-others-msg:after {
  /* 箭头靠左边 */
  content: '';
  position: absolute;
  width: 0;
  height: 0;
  top: 13px;
  left: -10px;
  border-top: 10px solid transparent;
  border-bottom: 10px solid transparent;
  border-right: 10px solid #FFFFFF;

}

.msg-null {
  width: 140px;
  height: 41px;
  background-color: #FFFFFF;
  position: relative;
  left: 132px;
  bottom: 60px;
  border-radius: 5px;
  // border: 2px solid #E5E5E5;
  display: flex;
  align-content: center;
  justify-content: center;
  flex-wrap: wrap;
  font-size: 14px;
}

.msg-null:after {
  /* 箭头靠下边 */
  content: "";
  position: absolute;
  width: 0;
  height: 0;
  top: 40px;
  left: 80px;
  border-top: 10px solid #FFFFFF;
  border-bottom: 10px solid transparent;
  border-right: 10px solid transparent;
  border-left: 10px solid transparent;

}


//以下是代码格式处理的样式

::v-deep(.li-list) {
  list-style: inside !important;
  //list-style: decimal !important;

}

::v-deep(.pre-out) {
  padding: 0;
  //overflow-x: hidden;
  overflow-x: scroll;
}

::v-deep(.pre) {
  max-width: 570px;
  padding: 0;
  margin-bottom: 0;
  //overflow-x: hidden;
  overflow-x: scroll;

  .header {
    background-color: #409eff;
    color: white;
    height: 30px;
    display: flex;
    justify-content: flex-end;
    padding-top: 5px;


    .language {
    }

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

.clear-msg {
  font-size: large;
  margin-left: 10px;
  cursor: pointer; /* 设置为手型 */
}

.clear-msg:hover {
  color: red;
  cursor: pointer; /* 设置鼠标悬浮为手型 */
}

::v-deep(.katex-html) {
  color: #7B7C7C;
  font-size: smaller;
}

::v-deep(.nav-ul) {
  border-right: 1px solid #FFFFFF;
  margin-top: 12px;
  margin-left: 0 !important;
  padding-left: 10px;
  padding-right: 2px;

  .nav-li {
    margin: 1.0px 0;
    text-align: right;
    margin-right: 3px;
  }
}

.content-msg-common ::v-deep(ul) {
  margin-left: 20px;
}

.content-msg-common ::v-deep(ol) {
  margin-left: 20px;
}

::v-deep(.katex) {
  margin: 10px;
  display: flex;
  flex-direction: column;
  align-content: center;
  flex-wrap: wrap;
  align-items: center;
  font-size: larger;
}


</style>