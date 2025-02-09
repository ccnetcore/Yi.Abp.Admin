
<template>
  <canvas id="starfield"></canvas>
</template>
<script setup>
import {onMounted} from "vue";
const props = defineProps(['speed','number'])

onMounted(()=>{
  startAnimate();
})


let canvas={};
let ctx={};
let stars = [];
let centerX=0;
let centerY=0;
const startAnimate=()=>{
  canvas = document.getElementById('starfield');
  ctx = canvas.getContext('2d');
  canvas.width = window.innerWidth;
  canvas.height = window.innerHeight;
  centerX = canvas.width / 2;
  centerY = canvas.height / 2;

  window.addEventListener('resize', () => {
    canvas.width = window.innerWidth;
    canvas.height = window.innerHeight;
  });

  createStars(props.number??1000);
  animate();
}

function createStars(numStars) {
  for (let i = 0; i < numStars; i++) {
    stars.push({
      x: (Math.random() - 0.5) * canvas.width,
      y: (Math.random() - 0.5) * canvas.height,
      z: Math.random() * canvas.width
    });
  }
}

function drawStars() {
  ctx.fillStyle = '#f0f2f5';
  ctx.fillRect(0, 0, canvas.width, canvas.height);

  ctx.fillStyle = 'black';
  stars.forEach(star => {
    const k = 128 / star.z;
    const px = star.x * k + centerX;
    const py = star.y * k + centerY;

    ctx.beginPath();
    ctx.arc(px, py, 1.5 * k, 0, Math.PI * 2);
    ctx.fill();

    star.z -=props.speed??0.1;

    if (star.z <= 0) {
      star.z = canvas.width;
      star.x = (Math.random() - 0.5) * canvas.width;
      star.y = (Math.random() - 0.5) * canvas.height;
    }
  });
}

function animate() {
  drawStars();
  requestAnimationFrame(animate);
}



</script>
<style scoped>

#starfield {
  position: fixed;
  top: 0;
  left: 0;
  z-index: -1;
}
</style>