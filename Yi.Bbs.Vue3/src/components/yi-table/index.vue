<template>
  <div class="yi-table">
    <el-table
      ref="tableRef"
      :data="tableData"
      v-bind="_options"
      header-row-class-name="header-row-class-name"
      @selection-change="handleSelectionChange"
      @row-click="handleRowClick"
      @cell-click="handleCellClick"
      @sort-change="handleSortChange"
    >
      <template v-for="(col, index) in columns">
        <!---复选框, 序号 (START)-->
        <el-table-column
          :key="index"
          v-if="
            col.type === 'index' ||
            col.type === 'selection' ||
            col.type === 'expand'
          "
          :sortable="col.sortable"
          :prop="col.prop"
          :align="col.align"
          :label="col.label"
          :type="col.type"
          :index="indexMethod"
          :width="col.width"
          v-bind="col"
        />

        <!---复选框, 序号 (END)-->
        <TableColumn
          :key="index + '_key'"
          :col="col"
          v-else
          @command="handleAction"
        >
          <template #default="{ slotName, row, index }">
            <slot :name="slotName" :row="row" :index="index" />
          </template>
          <!-- 自定义表头插槽 -->
          <template #customHeader="{ slotName, column, index }">
            <slot :name="slotName" :column="column" :index="index" />
          </template>
        </TableColumn>
      </template>
    </el-table>
  </div>
  <!-- 分页器 -->
  <div
    v-if="_options.showPagination"
    class="table-pagination"
    :class="[`pagination_${_options.paginationPosition}`]"
  >
    <el-pagination
      v-bind="_paginationConfig"
      @size-change="pageSizeChange"
      @current-change="currentPageChange"
    />
  </div>
</template>
<script setup name="YiTable">
import { computed, ref } from "vue";
import TableColumn from "./TableColumn.vue";

const props = defineProps({
  tableData: {
    //  table的数据
    type: Array,
    default: () => {
      return [];
    },
  },
  columns: {
    type: Array,
    default: () => {
      return [];
    },
  },
  options: {
    type: Object,
    default: () => {
      return {};
    },
  },
});
const tableRef = ref();
// 设置option默认值，如果传入自定义的配置则合并option配置项
const _options = computed(() => {
  const option = {
    stripe: false,
    tooltipEffect: "dark",
    showHeader: true,
    showPagination: false,
    paginationPosition: "right",
    rowStyle: () => "cursor:pointer", // 行样式
    // headerRowClassName: 'table-header-row',
  };
  return Object.assign(option, props?.options);
});
// 合并分页配置
const _paginationConfig = computed(() => {
  const config = {
    total: 0,
    currentPage: 1,
    pageSize: 10,
    pageSizes: [10, 20, 50],
    background: true,
    layout: "total, sizes, prev, pager, next, jumper",
  };
  return Object.assign(config, _options.value.paginationConfig);
});

const emit = defineEmits([
  "selection-change", // 当选择项发生变化时会触发该事件
  "row-click", // 当某一行被点击时会触发该事件
  "cell-click", // 当某个单元格被点击时会触发该事件
  "command", // 按钮组事件
  "size-change", // pageSize事件
  "current-change", // currentPage按钮组事件
  "pagination-change", // currentPage或者pageSize改变触发
  "sort-change", // 列排序发生改变触发
]);
// 自定义索引
const indexMethod = (index) => {
  const tabIndex =
    index +
    (_paginationConfig.value.currentPage - 1) *
      _paginationConfig.value.pageSize +
    1;
  return tabIndex;
};
// 切换pageSize
const pageSizeChange = (pageSize) => {
  emit("size-change", pageSize);
  emit("pagination-change", 1, pageSize);
};
// 切换currentPage
const currentPageChange = (currentPage) => {
  emit("current-change", currentPage);
  emit("pagination-change", currentPage, _paginationConfig.value.pageSize);
};
// 按钮组事件
const handleAction = (command, row) => {
  emit("command", command, row);
};
// 多选事件
const handleSelectionChange = (val) => {
  emit("selection-change", val);
};
// 当某一行被点击时会触发该事件
const handleRowClick = (row, column, event) => {
  emit("row-click", row, column, event);
};
// 当某个单元格被点击时会触发该事件
const handleCellClick = (row, column, cell, event) => {
  emit("cell-click", row, column, cell, event);
};
/**
 *  当表格的排序条件发生变化的时候会触发该事件
 * 在列中设置 sortable 属性即可实现以该列为基准的排序， 接受一个 Boolean，默认为 false。
 * 可以通过 Table 的 default-sort 属性设置默认的排序列和排序顺序。
 * 如果需要后端排序，需将 sortable 设置为 custom，同时在 Table 上监听 sort-change 事件，
 * 在事件回调中可以获取当前排序的字段名和排序顺序，从而向接口请求排序后的表格数据。
 */
const handleSortChange = ({ column, prop, order }) => {
  emit("sort-change", { column, prop, order });
};
// 暴露给父组件参数和方法，如果外部需要更多的参数或者方法，都可以从这里暴露出去。
defineExpose({ element: tableRef });
</script>
<style lang="scss" scoped>
:deep(.el-image__inner) {
  transition: all 0.3s;
  cursor: pointer;
  &:hover {
    transform: scale(1.2);
  }
}

.yi-table {
  flex: 1;
  position: relative;
  overflow: hidden;
  .el-table {
    height: 100%;
    position: absolute;
  }
}

.table-pagination {
  margin-top: 20px;
  display: flex;
  align-items: center;
}
.table-pagination.pagination_left {
  justify-content: flex-start;
}
.table-pagination.pagination_center {
  justify-content: center;
}
.table-pagination.pagination_right {
  justify-content: flex-end;
}
</style>
