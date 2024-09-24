<template>
	<h3 class="title">数据表选择</h3>
	<el-menu default-active="2" class="el-menu-vertical-demo">
		<el-menu-item v-for="(item,i) in dataList" :index="'data-'+i" :key="i" @click="menuClick(item)">
			<el-icon>
				<setting />
			</el-icon>
			<span>{{item.name}}</span>
		</el-menu-item>
	</el-menu>
	<el-row>
		<el-button-group>
			<el-button type="primary" @click="Prev" :icon="ArrowLeft">Previous Page</el-button>
			<el-button type="primary" @click="Next">
				Next Page<el-icon class="el-icon--right">
					<ArrowRight />
				</el-icon>
			</el-button>
		</el-button-group>
	</el-row>
</template>

<script lang="ts" setup>
	import { reactive, ref } from 'vue'
	import {
		listData
	} from "@/api/code/tableApi";

	const emit = defineEmits(['selectTable']);
	const total = ref(0);
	const dataList = ref([]);

	const data = ref({
		form: {},
		queryParams: {
			skipCount: 1,
			maxResultCount: 10,
			name: undefined
		},
		rules: {
			name: [{
				required: true,
				message: "表名称不能为空",
				trigger: "blur"
			}],
		},
	});

	function menuClick(item) {
		emit('selectTable', item)
	}

	//简版分页 上一页
	function Prev() {
		data.value.queryParams.skipCount--;
		if (data.value.queryParams.skipCount <= 1)
			data.value.queryParams.skipCount = 1;
		GetTableDataList();
	}
	//简版分页 下一页
	function Next() {
		data.value.queryParams.skipCount++;
		var totalMax = total.value % data.value.queryParams.maxResultCount == 0
			? total.value / data.value.queryParams.maxResultCount
			: total.value / data.value.queryParams.maxResultCount + 1;
		if (data.value.queryParams.skipCount >= totalMax) {
			data.value.queryParams.skipCount = parseInt(totalMax);
		}
		GetTableDataList();
	}
	//查询 Table 列表
	function GetTableDataList() {
		listData(data.value.queryParams).then(
			(response) => {
				dataList.value = response.data.items;
				total.value = response.data.totalCount;
			}
		);
	}
	GetTableDataList();
	console.log(dataList);
</script>
<style scoped>
	.el-menu-item {
		justify-content: center;
		max-height: 1000px;
	}

	.title {
		text-align: center;
	}
</style>