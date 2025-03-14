<script setup>
import AvatarInfo from "@/components/AvatarInfo.vue";
import { ref, onMounted } from "vue";

const props = defineProps([
    "goldUserInfo",
    "silverUserInfo",
    "bronzeUserInfo",
]);

const isVisible = ref(false);

onMounted(() => {
  setTimeout(() => {
    isVisible.value = true;
  }, 300);
});
</script>
<template>
    <div class="podium-container">
        <div class="podium" :class="{ 'visible': isVisible }">
            <div class="avatar silver-position">
                <div class="avatar-wrapper">
                    <AvatarInfo style="display: flex;" :userInfo="{nick:'123'}" :isSelf="true" />
                </div>
                <div class="tier silver">
                    <h2>第二</h2>
                    <h3>{{ silverUserInfo?.money }}</h3>
                </div>
            </div>

            <div class="avatar gold-position">
                <div class="avatar-wrapper">
                    <AvatarInfo :userInfo="goldUserInfo" :isSelf="true" :size="50" />
                </div>
                <div class="tier gold">
                    <h2>第一</h2>
                    <h3>{{ goldUserInfo?.money }}</h3>
                </div>
            </div>

            <div class="avatar bronze-position">
                <div class="avatar-wrapper">
                    <AvatarInfo :userInfo="bronzeUserInfo" :isSelf="true" />
                </div>
                <div class="tier bronze">
                    <h2>第三</h2>
                    <h3>{{ bronzeUserInfo?.money }}</h3>
                </div>
            </div>
        </div>
    </div>
</template>
<style scoped>
h2{
    margin: 10px;
    text-shadow: 2px 2px 4px rgba(0, 0, 0, 0.3);
}
h3{
    margin: 10px;
    font-size: 1.5rem;
    text-shadow: 2px 2px 4px rgba(0, 0, 0, 0.3);
}
.podium-container {
    position: relative;
    margin: 40px 0;
}
.avatar {
    display: flex;
    justify-content: center;
    flex-direction: column;
    position: relative;
    transition: all 0.8s cubic-bezier(0.175, 0.885, 0.32, 1.275);
}

.visible .gold-position {
    transform: translateY(0);
    opacity: 1;
    transition-delay: 0.3s;
}

.visible .silver-position {
    transform: translateY(0);
    opacity: 1;
    transition-delay: 0.6s;
}

.visible .bronze-position {
    transform: translateY(0);
    opacity: 1;
    transition-delay: 0.9s;
}

.medal-icon {
    position: absolute;
    width: 30px;
    height: 30px;
    border-radius: 50%;
    display: flex;
    justify-content: center;
    align-items: center;
    color: white;
    font-weight: bold;
    top: -10px;
    right: 55px;
    z-index: 2;
    box-shadow: 0 4px 8px rgba(0, 0, 0, 0.2);
}

.gold-medal {
    background: #ffd700;
}

.silver-medal {
    background: #c0c0c0;
}

.bronze-medal {
    background: #cd7f32;
}

.crown {
    position: absolute;
    top: -30px;
    left: 50%;
    z-index: 10;
    transform: translateX(-53px);
}

.gold-crown:before,
.gold-crown:after {
    content: "";
    position: absolute;
    bottom: 0;
    width: 0;
    height: 0;
}

.gold-crown {
    width: 30px;
    height: 20px;
    background: #ffd700;
    border-radius: 4px 4px 0 0;
}

.gold-crown:before {
    left: -12px;
    border-left: 18px solid transparent;
    border-right: 0px solid transparent;
    border-bottom: 30px solid #ffd700;
}

.gold-crown:after {
    right: -12px;
    border-left: 0px solid transparent;
    border-right: 18px solid transparent;
    border-bottom: 30px solid #ffd700;
}

.podium {
    justify-content: center;
    display: flex;
    align-items: flex-end;
    position: relative;
    padding: 20px 0;
}

.tier {
    flex-direction: column;
    width: 150px;
    height: 200px;
    display: flex;
    justify-content: center;
    align-items: center;
    margin: 5px 40px 40px 40px;
    border-radius: 20px;
    color: white;
    font-size: 24px;
    position: relative;
    transform: perspective(1000px) rotateX(5deg);
    transition: all 0.3s ease;
}

.gold:hover {
    box-shadow: 0 0 40px 20px rgba(255, 215, 0, 0.7);
    transform: perspective(1000px) rotateX(0deg) scale(1.05);
}

.gold {
    background: linear-gradient(to bottom, #ffd700 0%, #ffcc00 100%);
    height: 250px;
    box-shadow: 0 8px 20px rgba(255, 215, 0, 0.5);
    z-index: 3;
}
.silver:hover {
    box-shadow: 0 0 20px 10px rgba(192, 192, 192, 0.7);
    transform: perspective(1000px) rotateX(0deg) scale(1.05);
}

.silver {
    background: linear-gradient(to bottom, #c0c0c0 0%, #d3d3d3 100%);
    height: 200px;
    box-shadow: 0 6px 18px rgba(192, 192, 192, 0.5);
    z-index: 2;
}
.bronze:hover {
    box-shadow: 0 0 20px 10px rgba(205, 127, 50, 0.7);
    transform: perspective(1000px) rotateX(0deg) scale(1.05);
}
.bronze {
    background: linear-gradient(to bottom, #cd7f32 0%, #a0522d 100%);
    height: 170px;
    box-shadow: 0 4px 16px rgba(205, 127, 50, 0.5);
    z-index: 1;
}
.avatar-wrapper {
    display: flex;
    justify-content: center;
    align-items: center;
    margin-bottom: 10px;
    z-index: 10;
    position: relative;
}
</style>