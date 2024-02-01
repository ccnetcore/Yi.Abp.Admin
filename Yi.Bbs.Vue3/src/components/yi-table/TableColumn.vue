<script setup>
import dayjs from "dayjs";
defineProps({
  col: {
    //  Column
    type: Object,
    default: () => {
      return {};
    },
  },
});
const emit = defineEmits(["command"]);
// 按钮组事件
const handleAction = (command, scope) => {
  emit("command", command, scope.row);
};
</script>
<template>
  <!-- 如果有配置多级表头的数据，则递归该组件 -->
  <template v-if="col.children?.length">
    <el-table-column
      :label="col.label"
      :width="col.width"
      :align="col.align"
      v-bind="col"
    >
      <TableColumn
        v-for="(item, index) in col.children"
        :col="item"
        :key="index"
      />
    </el-table-column>
  </template>
  <el-table-column
    v-else
    :show-overflow-tooltip="col.showOverflowTooltip"
    :sortable="col.sortable"
    :prop="col.prop"
    :align="col.align"
    :label="col.label"
    :width="col.width"
    v-bind="col"
  >
    <template #default="{ row, $index }">
      <!---图片 (START)-->
      <!-- 如需更改图片size，可自行配置参数 -->
      <el-image
        v-if="col.type === 'image'"
        preview-teleported
        :hide-on-click-modal="true"
        :preview-src-list="[row[col.prop]]"
        :src="row[col.prop]"
        fit="cover"
        class="w-9 h-9 rounded-lg"
      />
      <!---图片 (END)-->
      <!--- 格式化日期 (本项目日期是时间戳，这里日期格式化可根据你的项目来更改) (START)-->
      <template v-else-if="col.type === 'date'">
        <!---十位数时间戳-->
        <span v-if="String(row[col.prop])?.length <= 10">
          {{
            row[col.prop]
              ? dayjs.unix(row[col.prop]).format(col.dateFormat ?? "YYYY-MM-DD")
              : ""
          }}
        </span>
        <!---十三位数时间戳-->
        <span v-else>{{
          dayjs(row[col.prop]).format(col.dateFormat ?? "YYYY-MM-DD")
        }}</span>
      </template>
      <!--- 格式化日期 (本项目日期是时间戳，这里日期格式化可根据你的项目来更改) (END)-->
      <!-- 如果传递按钮数组，就展示按钮组 START-->
      <!-- v-permission 给按钮添加权限 传[]则不设置权限-->
      <el-button-group v-else-if="col.buttons?.length">
        <el-button
          v-for="(btn, index) in col.buttons"
          :key="index"
          :size="btn.size || 'small'"
          :type="btn.type"
          @click="handleAction(btn.command, { row, $index })"
          :disabled="btn.disabled || false"
          v-permission="btn.permission || []"
          >{{ btn.name }}</el-button
        >
      </el-button-group>
      <!-- 如果传递按钮数组，就展示按钮组 END-->
      <!-- render函数 (START) -->
      <!-- 使用内置的component组件可以支持h函数渲染和txs语法 -->
      <component
        v-else-if="col.render"
        :is="col.render"
        :row="row"
        :index="$index"
      />
      <!-- render函数 (END) -->
      <!-- 自定义slot (START) -->
      <slot
        v-else-if="col.slot"
        :slotName="col.slot"
        :row="row"
        :index="$index"
      ></slot>
      <!-- 自定义slot (END) -->
      <!-- 默认渲染 (START) -->
      <span v-else>{{ row[col.prop] }}</span>
      <!-- 默认渲染 (END) -->
    </template>
    <!-- 自定义表头 -->
    <template #header="{ column, $index }">
      <component
        v-if="col.headerRender"
        :is="col.headerRender"
        :column="column"
        :index="$index"
      />
      <slot
        v-else-if="col.headerSlot"
        name="customHeader"
        :slotName="col.headerSlot"
        :column="column"
        :index="$index"
      ></slot>
      <span v-else>{{ column.label }}</span>
    </template>
  </el-table-column>
</template>
