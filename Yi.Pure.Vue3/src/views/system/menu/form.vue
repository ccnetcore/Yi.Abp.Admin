<script setup lang="ts">
import { ref } from "vue";
import ReCol from "@/components/ReCol";
import { formRules } from "./utils/rule";
import { FormProps } from "./utils/types";
import { transformI18n } from "@/plugins/i18n";
import { IconSelect } from "@/components/ReIcon";
import Segmented from "@/components/ReSegmented";
import ReAnimateSelector from "@/components/ReAnimateSelector";
import {
  menuTypeOptions,
  showLinkOptions,
  fixedTagOptions,
  keepAliveOptions,
  hiddenTagOptions,
  showParentOptions,
  frameLoadingOptions,
  isLinkOptions,
  stateOptions
} from "./utils/enums";

const props = withDefaults(defineProps<FormProps>(), {
  formInline: () => ({
    menuType: 0,
    higherMenuOptions: [],
    parentId: "00000000-0000-0000-0000-000000000000",
    menuName: "",
    router: "",
    component: "",
    orderNum: 0,
    menuIcon: "",
    permissionCode: "",
    isShow: true,
    isLink: false,
    state: true,
    routerName: "",
    menuSource: "Pure"
  })
});

const ruleFormRef = ref();
const newFormInline = ref(props.formInline);

function getRef() {
  return ruleFormRef.value;
}

defineExpose({ getRef });
</script>

<template>
  <el-form
    ref="ruleFormRef"
    :model="newFormInline"
    :rules="formRules"
    label-width="82px"
  >
    <el-row :gutter="30">
      <re-col>
        <el-form-item label="菜单类型">
          <Segmented
            v-model="newFormInline.menuType"
            :options="menuTypeOptions"
          />
        </el-form-item>
      </re-col>

      <re-col>
        <el-form-item label="上级菜单">
          <el-cascader
            v-model="newFormInline.parentId"
            class="w-full"
            :options="newFormInline.higherMenuOptions"
            :props="{
              value: 'id',
              label: 'menuName',
              emitPath: false,
              checkStrictly: true
            }"
            clearable
            filterable
            placeholder="请选择上级菜单"
          >
            <template #default="{ node, data }">
              <span>{{ transformI18n(data.menuName) }}</span>
              <span v-if="!node.isLeaf"> ({{ data.children.length }}) </span>
            </template>
          </el-cascader>
        </el-form-item>
      </re-col>
      <re-col
        v-show="newFormInline.menuType !== 2"
        :value="24"
        :xs="24"
        :sm="24"
      >
        <el-form-item label="菜单图标">
          <IconSelect v-model="newFormInline.menuIcon" class="w-full" />
        </el-form-item>
      </re-col>
      <re-col :value="12" :xs="24" :sm="24">
        <el-form-item label="菜单名称" prop="menuName">
          <el-input
            v-model="newFormInline.menuName"
            clearable
            placeholder="请输入菜单名称"
          />
        </el-form-item>
      </re-col>

      <re-col :value="12" :xs="24" :sm="24">
        <el-form-item label="菜单排序" prop="orderNum">
          <el-input-number
            v-model="newFormInline.orderNum"
            class="!w-full"
            :min="1"
            :max="9999"
            controls-position="right"
          />
        </el-form-item>
      </re-col>

      <re-col
        v-show="newFormInline.menuType !== 2"
        :value="12"
        :xs="24"
        :sm="24"
      >
        <el-form-item label="是否为外链">
          <Segmented
            :modelValue="newFormInline.isLink ? false : true"
            :options="isLinkOptions"
            @change="
              ({ option: { value } }) => {
                newFormInline.isLink = value;
              }
            "
          />
        </el-form-item>
      </re-col>

      <re-col
        v-show="newFormInline.menuType === 1"
        :value="12"
        :xs="24"
        :sm="24"
      >
        <el-form-item label="组件路径">
          <el-input
            v-model="newFormInline.component"
            clearable
            placeholder="请输入组件路径"
          />
        </el-form-item>
      </re-col>
      <re-col v-if="newFormInline.menuType !== 2" :value="12" :xs="24" :sm="24">
        <el-form-item label="路由路径" prop="router">
          <el-input
            v-model="newFormInline.router"
            clearable
            placeholder="请输入路由路径"
          />
        </el-form-item>
      </re-col>

      <re-col v-if="newFormInline.menuType !== 0" :value="12" :xs="24" :sm="24">
        <!-- 按钮级别权限设置 -->
        <el-form-item label="权限标识" prop="permissionCode">
          <el-input
            v-model="newFormInline.permissionCode"
            clearable
            placeholder="请输入权限标识"
          />
        </el-form-item>
      </re-col>

      <re-col
        v-show="newFormInline.menuType !== 'Component'"
        :value="12"
        :xs="24"
        :sm="24"
      >
        <el-form-item label="是否显示">
          <Segmented
            :modelValue="newFormInline.isShow ? false : true"
            :options="showLinkOptions"
            @change="
              ({ option: { value } }) => {
                newFormInline.isShow = value;
              }
            "
          />
        </el-form-item>
      </re-col>
      <re-col :value="12" :xs="24" :sm="24">
        <el-form-item label="是否启用">
          <Segmented
            :modelValue="newFormInline.state ? false : true"
            :options="stateOptions"
            @change="
              ({ option: { value } }) => {
                newFormInline.state = value;
              }
            "
          />
        </el-form-item>
      </re-col>
    </el-row>
  </el-form>
</template>
