<script setup lang="ts">
  import type { VbenFormProps } from '@vben/common-ui';
  
  import type { VxeGridProps } from '#/adapter/vxe-table';
  import type { Dept } from '#/api/system/dept/model';
  
  import { nextTick } from 'vue';
  
  import { Page, useVbenDrawer } from '@vben/common-ui';
  import { eachTree, getVxePopupContainer } from '@vben/utils';
  
  import { Popconfirm, Space } from 'ant-design-vue';
  
  import { useVbenVxeGrid } from '#/adapter/vxe-table';
  import { deptList, deptRemove } from '#/api/system/dept';
  
  import { columns, querySchema } from './data';
  import deptDrawer from './dept-drawer.vue';
  
  const formOptions: VbenFormProps = {
    commonConfig: {
      labelWidth: 80,
      componentProps: {
        allowClear: true,
      },
    },
    schema: querySchema(),
    wrapperClass: 'grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4',
  };
  
  // 空GUID，用于判断根节点
  const EMPTY_GUID = '00000000-0000-0000-0000-000000000000';

  const gridOptions: VxeGridProps = {
    columns,
    height: 'auto',
    keepSource: true,
    pagerConfig: {
      enabled: false,
    },
    proxyConfig: {
      ajax: {
        query: async (_, formValues = {}) => {
          const resp = await deptList({
            ...formValues,
          });
          // 将根节点的 parentId 置为 null，以便 vxe-table 正确识别根节点
          const items = resp.map((item) => ({
            ...item,
            parentId: item.parentId === EMPTY_GUID ? null : item.parentId,
          }));
          return { items };
        },
        // 默认请求接口后展开全部 不需要可以删除这段
        querySuccess: () => {
          // 默认展开 需要加上标记
          // eslint-disable-next-line no-use-before-define
          eachTree(tableApi.grid.getData(), (item) => (item.expand = true));
          nextTick(() => {
            setExpandOrCollapse(true);
          });
        },
      },
    },
    rowConfig: {
      keyField: 'id',
    },
    treeConfig: {
      parentField: 'parentId',
      rowField: 'id',
      transform: true,
    },
    id: 'system-dept-index',
  };
  
  const [BasicTable, tableApi] = useVbenVxeGrid({
    formOptions,
    gridOptions,
    gridEvents: {
      cellDblclick: (e) => {
        const { row = {} } = e;
        if (!row?.children) {
          return;
        }
        const isExpanded = row?.expand;
        tableApi.grid.setTreeExpand(row, !isExpanded);
        row.expand = !isExpanded;
      },
      // 需要监听使用箭头展开的情况 否则展开/折叠的数据不一致
      toggleTreeExpand: (e) => {
        const { row = {}, expanded } = e;
        row.expand = expanded;
      },
    },
  });
  const [DeptDrawer, drawerApi] = useVbenDrawer({
    connectedComponent: deptDrawer,
  });
  
  function handleAdd() {
    drawerApi.setData({ update: false });
    drawerApi.open();
  }
  
function handleSubAdd(row: Dept) {
  const { id } = row;
  drawerApi.setData({ id, update: false });
  drawerApi.open();
}
  
async function handleEdit(record: Dept) {
  drawerApi.setData({ id: record.id, update: true });
  drawerApi.open();
}
  
async function handleDelete(row: Dept) {
  await deptRemove(row.id);
  await tableApi.query();
}
  
  /**
   * 全部展开/折叠
   * @param expand 是否展开
   */
  function setExpandOrCollapse(expand: boolean) {
    eachTree(tableApi.grid.getData(), (item) => (item.expand = expand));
    tableApi.grid?.setAllTreeExpand(expand);
  }
  </script>
  
  <template>
    <Page :auto-content-height="true">
      <BasicTable table-title="部门列表" table-title-help="双击展开/收起子菜单">
        <template #toolbar-tools>
          <Space>
            <a-button @click="setExpandOrCollapse(false)">
              {{ $t('pages.common.collapse') }}
            </a-button>
            <a-button @click="setExpandOrCollapse(true)">
              {{ $t('pages.common.expand') }}
            </a-button>
            <a-button
              type="primary"
              v-access:code="['system:dept:add']"
              @click="handleAdd"
            >
              {{ $t('pages.common.add') }}
            </a-button>
          </Space>
        </template>
        <template #action="{ row }">
          <Space>
            <ghost-button
              v-access:code="['system:dept:edit']"
              @click="handleEdit(row)"
            >
              {{ $t('pages.common.edit') }}
            </ghost-button>
            <ghost-button
              class="btn-success"
              v-access:code="['system:dept:add']"
              @click="handleSubAdd(row)"
            >
              {{ $t('pages.common.add') }}
            </ghost-button>
            <Popconfirm
              :get-popup-container="getVxePopupContainer"
              placement="left"
              title="确认删除？"
              @confirm="handleDelete(row)"
            >
              <ghost-button
                danger
                v-access:code="['system:dept:remove']"
                @click.stop=""
              >
                {{ $t('pages.common.delete') }}
              </ghost-button>
            </Popconfirm>
          </Space>
        </template>
      </BasicTable>
      <DeptDrawer @reload="tableApi.query()" />
    </Page>
  </template>
  