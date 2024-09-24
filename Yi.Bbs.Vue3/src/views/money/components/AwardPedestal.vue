<script setup>
import AvatarInfo from "@/components/AvatarInfo.vue";
const props = defineProps([
    "goldUserInfo",
    "silverUserInfo",
    "bronzeUserInfo",
]);
</script>
<template>
    <div class="podium">
        <div class="avatar">
            <AvatarInfo :userInfo="silverUserInfo" :isSelf="true" />
            <div class="tier silver">
                <h2>第二</h2>
                <h3>{{ silverUserInfo?.money }}</h3>
            </div>
        </div>

        <div class="avatar">
            <AvatarInfo :userInfo="goldUserInfo" :isSelf="true" :size="50" />
            <div class="tier gold">
                <h2>第一</h2>
                <h3>{{ goldUserInfo?.money }}</h3>
            </div>
        </div>

        <div class="avatar">
            <AvatarInfo :userInfo="bronzeUserInfo" :isSelf="true" />
            <div class="tier bronze">
                <h2>第三</h2>
                <h3>{{ bronzeUserInfo?.money }}</h3>
            </div>
        </div>
    </div>
</template>
<style scoped>
h2{
    margin: 10px;
}
h3{
    margin: 10px;
}
.avatar {
    display: flex;
    justify-content: center;
    flex-direction: column;
}

.podium {
    justify-content: center;
    display: flex;
    align-items: flex-end;
    position: relative;
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
}

.gold:hover {
            box-shadow: 0 0 20px 10px rgba(255, 215, 0, 0.7); /* 金色光辉效果 */
        }

.gold {
    background: linear-gradient(to bottom, #ffd700 0%, #ffcc00 100%);
    height: 250px;
    box-shadow: 0 8px 20px rgba(255, 215, 0, 0.5);
}

.silver {
    background: linear-gradient(to bottom, #c0c0c0 0%, #d3d3d3 100%);
    height: 200px;
    box-shadow: 0 6px 18px rgba(192, 192, 192, 0.5);
}

.bronze {
    background: linear-gradient(to bottom, #cd7f32 0%, #a0522d 100%);
    height: 170px;
    box-shadow: 0 4px 16px rgba(205, 127, 50, 0.5);
}

</style>