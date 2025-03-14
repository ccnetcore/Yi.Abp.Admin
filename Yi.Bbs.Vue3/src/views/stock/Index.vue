<template>
  <div class="stock-dashboard">
   <!-- 顶部选择器区域 -->
    <div class="stock-header">
      <div class="title-area">
        <h2 class="title">意社区股市v1.0 <span class="title-desc">本模块全部由AI 100% 调教生成</span></h2>
      </div>
      
      <div class="selector-area">
        <div class="user-money">当前钱钱: <span class="money-value">{{ userInfo.money || 0 }}</span></div>
        
        <el-select 
          v-model="currentStock" 
          placeholder="选择股票" 
          @change="changeStock" 
          size="small"
          :loading="isLoadingStockList"
          :disabled="isLoadingStockList"
        >
          <el-option 
            v-for="item in stockList" 
            :key="item.id" 
            :label="item.name" 
            :value="item.id">
            <span>{{ item.name }}</span>
            <span class="stock-code">{{ item.code }}</span>
          </el-option>
        </el-select>
        
        <el-button 
          type="primary" 
          size="small" 
          class="return-button" 
          @click="returnToHome"
        >
          <i class="el-icon-back"></i> 返回社区
        </el-button>
      </div>
    </div>
    
    <!-- 主内容区域 -->
    <div class="main-content">
      <!-- 左侧新闻区域 -->
      <div class="news-section">
        <div class="section-header">
          <span class="icon"><i class="el-icon-news"></i></span>
          <h3>最新市场动态</h3>
        </div>
        <div class="news-list">
          <div v-if="isLoadingNews && newsList.length === 0" class="loading-news">
            <el-skeleton :rows="5" animated />
          </div>
          <template v-else>
            <div 
              v-for="news in newsList" 
              :key="news.id" 
              class="news-item"
              @click="showNewsDetail(news)"
            >
              <div class="news-date">
                {{ dayjs(news.publishTime).format('YYYY-MM-DD') }}
                <span class="news-source" v-if="news.source">· {{ news.source }}</span>
              </div>
              <div class="news-title">{{ news.title }}</div>
            </div>
            
            <div v-if="newsList.length === 0" class="empty-news">
              暂无市场动态
            </div>
            
            <div v-if="newsTotal > newsList.length" class="news-more">
              <el-button 
                type="primary" 
                size="small" 
                text 
                @click="loadMoreNews"
                :loading="isLoadingNews"
              >
                加载更多
              </el-button>
            </div>
          </template>
        </div>
      </div>
      
      <!-- 中间股票看板区域 -->
      <div class="stock-panel">
        <div class="stock-info">
          <h3>{{ currentStockInfo.name }} ({{ currentStockInfo.code }})</h3>
          <div class="price-info">
            <div class="current-price" :class="{'price-up': priceChange > 0, 'price-down': priceChange < 0}">
              {{ currentStockInfo.price.toFixed(2) }}
            </div>
            <div class="price-change" :class="{'price-up': priceChange > 0, 'price-down': priceChange < 0}">
              {{ priceChange > 0 ? '+' : '' }}{{ priceChange.toFixed(2) }} ({{ priceChangePercent }}%)
            </div>
          </div>
        </div>
        
        <!-- 股票图表 -->
        <div class="stock-chart">
          <div class="chart-header">
            <el-button 
              type="primary" 
              circle 
              size="small" 
              class="refresh-button" 
              @click="refreshStockChart"
              :loading="isLoadingChart"
            >
            <el-icon><Refresh /></el-icon>
            </el-button>
          </div>
          <div v-if="isLoadingChart" class="loading-chart">
            <el-skeleton animated />
          </div>
          <template v-else>
            <StockChart v-if="stockChartData.length > 0" :stockData="stockChartData" />
            <el-empty v-else description="暂无股票数据" />
          </template>
        </div>
        
        <!-- 交易操作区 -->
        <div class="trade-actions">
          <el-button type="success" circle class="circle-button" @click="buyStock">买入</el-button>
          <el-input-number v-model="tradeAmount" :min="10" :max="1000" label="数量" controls-position="right"></el-input-number>
          <el-button type="danger" circle class="circle-button" @click="sellStock">卖出</el-button>
        </div>
      </div>
      
      <!-- 右侧交易记录 -->
      <div class="trade-history">
        <div class="section-header">
          <span class="icon"><i class="el-icon-time"></i></span>
          <h3>交易记录</h3>
        </div>
        <div class="history-list">
          <div v-if="isLoadingTradeHistory" class="loading-history">
            <el-skeleton :rows="5" animated />
          </div>
          
          <template v-else>
            <div v-if="tradeHistory.length === 0" class="empty-history">
              暂无交易记录
            </div>
            
            <div v-for="(record, index) in tradeHistory" :key="index" class="history-item">
              <div class="history-header">
                <span class="stock-name">{{ record.stockName }}</span>
                <span class="time">{{ record.time }}</span>
              </div>
              <div class="history-details">
                <span class="type" :class="record.type === 'buy' ? 'buy-type' : 'sell-type'">
                  {{ record.type === 'buy' ? '买入' : '卖出' }}
                </span>
                <span class="amount">{{ record.amount }}股</span>
                <span class="price">¥{{ record.price }}</span>
                <span class="total">总额: ¥{{ record.totalAmount }}</span>
              </div>
            </div>
          </template>
        </div>
      </div>
    </div>
    
    <!-- 底部持仓信息 -->
    <div class="portfolio">
      <div class="section-header">
        <span class="icon"><i class="el-icon-wallet"></i></span>
        <h3>我的持仓</h3>
        <span class="total-value">持有钱钱: ¥{{ totalAssets }}</span>
      </div>
      
      <div v-if="isLoadingPortfolio" class="loading-portfolio">
        <el-skeleton :rows="3" animated />
      </div>
      
      <template v-else>
        <el-empty v-if="portfolioList.length === 0" description="暂无持仓" />
        
        <el-table v-else :data="portfolioList" stripe style="width: 100%">
          <el-table-column prop="name" label="股票名称"></el-table-column>
          <el-table-column prop="code" label="代码"></el-table-column>
          <el-table-column prop="amount" label="持有数量"></el-table-column>
          <el-table-column prop="buyPrice" label="买入均价"></el-table-column>
          <el-table-column prop="currentPrice" label="当前价格"></el-table-column>
          <el-table-column prop="profit" label="盈亏">
            <template #default="scope">
              <span :class="parseFloat(scope.row.profit) >= 0 ? 'price-up' : 'price-down'">
                {{ parseFloat(scope.row.profit) >= 0 ? '+' : '' }}{{ scope.row.profit }}
              </span>
            </template>
          </el-table-column>
        </el-table>
      </template>
    </div>

    <!-- 添加新闻详情弹窗 -->
    <el-dialog
      v-model="newsDialogVisible"
      :title="currentNewsDetail.title"
      width="50%"
      class="news-detail-dialog"
    >
      <div class="news-detail-header">
        <span class="news-detail-time">{{ dayjs(currentNewsDetail.publishTime).format('YYYY-MM-DD HH:mm:ss') }}</span>
        <span class="news-detail-source" v-if="currentNewsDetail.source">来源: {{ currentNewsDetail.source }}</span>
      </div>
      <div class="news-detail-content" style=" font-size: large">{{ currentNewsDetail.content }}</div>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, reactive } from 'vue';
import StockChart from './components/StockChart.vue';
import { getStockNews, getUserHoldings, getUserTransactions, getStockPriceRecords, getStockMarkets, buyStock as buyStockApi, sellStock as sellStockApi } from '@/apis/stockApi.js';
import { getBbsUserProfile } from '@/apis/userApi.js';
import { dayjs } from 'element-plus';
import { ElMessage, ElMessageBox } from 'element-plus';
import { useRouter } from 'vue-router';
import useUserStore from "@/stores/user";

// 获取路由器实例
const router = useRouter();
const userStore = useUserStore();
const userInfo = ref({});

// 返回首页函数
const returnToHome = () => {
  router.push('/index');
};

// 获取用户信息，包括钱钱
const loadUserInfo = async () => {
  try {
    const { data } = await getBbsUserProfile(userStore.id);
    userInfo.value = data;
  } catch (error) {
    console.error('获取用户信息失败:', error);
  }
};

// 股票列表数据
const stockList = ref([]);
const isLoadingStockList = ref(false);

// 当前选中的股票
const currentStock = ref('');

// 当前股票信息
const currentStockInfo = ref({
  name: '',
  code: '',
  price: 0,
  prevClose: 0
});

// 获取股市列表
const fetchStockMarkets = async () => {
  try {
    isLoadingStockList.value = true;
    const { data } = await getStockMarkets();
    
    // 将API返回的数据映射到组件所需的格式
    stockList.value = (data.items || []).map(item => ({
      id: item.id,
      name: item.marketName,
      code: item.marketCode,
      description: item.description,
      state: item.state
    }));
    
    // 如果有股票数据，默认选中第一个
    if (stockList.value.length > 0) {
      currentStock.value = stockList.value[0].id;
      currentStockInfo.value = {
        name: stockList.value[0].name,
        code: stockList.value[0].code,
        price: 0, // 将通过API更新
        prevClose: 0 // 将通过API更新
      };
      
      // 加载默认选中股票的数据
      await fetchStockPriceRecords();
    }
  } catch (error) {
    console.error('获取股市列表失败:', error);
  } finally {
    isLoadingStockList.value = false;
  }
};

// 价格变化
const priceChange = computed(() => {
  return parseFloat((currentStockInfo.value.price - currentStockInfo.value.prevClose).toFixed(2));
});

// 价格变化百分比
const priceChangePercent = computed(() => {
  return parseFloat(((priceChange.value / currentStockInfo.value.prevClose) * 100).toFixed(2));
});

// 交易数量
const tradeAmount = ref(10);

// 图表数据
const stockChartData = ref([]);
const isLoadingChart = ref(false);

// 获取股票价格记录
const fetchStockPriceRecords = async () => {
  try {
    isLoadingChart.value = true;
    
    // 计算时间范围: 今天的近5个小时
    const now = dayjs();
    const endTime = now.format('YYYY-MM-DD HH:00:00');
    const startTime = now.subtract(24, 'hour').format('YYYY-MM-DD HH:00:00');
    
    // 获取当前选中股票的ID
    // 注意：这里假设stockList中的id就是后端的stockId，如果不是需要调整
    const selectedStock = stockList.value.find(item => item.id === currentStock.value);
    const stockId = selectedStock ? selectedStock.id : '';
    
    if (!stockId) {
      console.error('未找到有效的股票ID');
      return;
    }
    
    const { data } = await getStockPriceRecords(stockId, startTime, endTime);
    // 将API返回的数据映射到图表所需的格式
    stockChartData.value = (data.items || []).map(item => ({
      date: dayjs(item.recordTime).format('MM-DD HH'),
      value: item.currentPrice
    }));
    
    // 如果有数据，更新当前股票信息
    if (stockChartData.value.length > 0) {
      const latestRecord = stockChartData.value[stockChartData.value.length - 1];
      currentStockInfo.value.price = latestRecord.value;
      // 如果只有一条记录，前收盘价设为当前价格，否则设为前一条记录的价格
      currentStockInfo.value.prevClose = stockChartData.value.length > 1 
        ? stockChartData.value[stockChartData.value.length - 2].value 
        : latestRecord.value;
    }
  } catch (error) {
    console.error('获取股票价格记录失败:', error);
  } finally {
    isLoadingChart.value = false;
  }
};

// 新闻列表查询参数
const newsQuery = reactive({
  skipCount: 1,
  maxResultCount: 10
});

// 新闻列表数据
const newsList = ref([]);
const newsTotal = ref(0);
const isLoadingNews = ref(false);

// 加载更多新闻
const loadMoreNews = async () => {
  newsQuery.skipCount += 1;
  await fetchNewsList(false);
};

// 获取新闻列表
const fetchNewsList = async (isReset = true) => {
  try {
    isLoadingNews.value = true;
    const { data } = await getStockNews(newsQuery);
    
    if (isReset) {
      newsList.value = data.items || [];
    } else {
      newsList.value = [...newsList.value, ...(data.items || [])];
    }
    
    newsTotal.value = data.totalCount || 0;
  } catch (error) {
    console.error('获取新闻列表失败:', error);
  } finally {
    isLoadingNews.value = false;
  }
};

// 交易记录
const tradeHistory = ref([]);
const isLoadingTradeHistory = ref(false);

// 获取交易记录
const fetchTradeHistory = async () => {
  try {
    isLoadingTradeHistory.value = true;
    const { data } = await getUserTransactions();
    
    // 将API返回的数据映射到组件所需的格式
    tradeHistory.value = (data.items || []).map(item => ({
      time: dayjs(item.creationTime).format('YYYY-MM-DD HH:mm:ss'),
      type: item.transactionType.toLowerCase(), // 转为小写以匹配现有的CSS类
      amount: item.quantity,
      price: item.price.toFixed(2),
      stockName: item.stockName,
      totalAmount: item.totalAmount.toFixed(2)
    }));
  } catch (error) {
    console.error('获取交易记录失败:', error);
  } finally {
    isLoadingTradeHistory.value = false;
  }
};

// 持仓列表
const portfolioList = ref([]);
const isLoadingPortfolio = ref(false);

// 获取持仓列表
const fetchPortfolioList = async () => {
  try {
    isLoadingPortfolio.value = true;
    const { data } = await getUserHoldings();
    
    // 将API返回的数据映射到组件所需的格式
    portfolioList.value = (data.items || []).map(item => ({
      name: item.stockName,
      code: item.stockCode,
      amount: item.quantity,
      buyPrice: item.costPrice.toFixed(2),
      currentPrice: item.currentPrice.toFixed(2),
      profit: item.profitLoss.toFixed(2)
    }));
  } catch (error) {
    console.error('获取持仓列表失败:', error);
  } finally {
    isLoadingPortfolio.value = false;
  }
};

// 总资产
const totalAssets = computed(() => {
  const stockValue = portfolioList.value.reduce((sum, stock) => {
    return sum + parseFloat(stock.currentPrice) * stock.amount;
  }, 0);
  return (stockValue + (userInfo.value.money || 0)).toFixed(2); // 使用API获取的钱钱，而不是固定值
});

// 切换股票
const changeStock = async (stockId) => {
  const stock = stockList.value.find(item => item.id === stockId);
  if (stock) {
    currentStockInfo.value = {
      name: stock.name,
      code: stock.code,
      price: 0, // 临时值，将通过API更新
      prevClose: 0 // 临时值，将通过API更新
    };
    
    // 更新图表数据
    await fetchStockPriceRecords();
    
    // 更新交易记录
    await fetchTradeHistory();
  }
};

// 买入股票
const buyStock = async () => {
  if (!currentStock.value) {
    ElMessage.warning('请先选择股票');
    return;
  }
  
  try {
    await ElMessageBox.confirm(
      `确定要买入 ${tradeAmount.value}股 ${currentStockInfo.value.name} 吗？`,
      '买入确认',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning',
      }
    );
    
    await buyStockApi({
      stockId: currentStock.value,
      quantity: tradeAmount.value
    });
    
    ElMessage.success(`已成功买入 ${tradeAmount.value}股 ${currentStockInfo.value.name}`);
    
    // 刷新数据
    await Promise.all([
      fetchTradeHistory(),
      fetchPortfolioList(),
      loadUserInfo() // 买入后更新用户钱钱
    ]);
  } catch (error) {
    if (error !== 'cancel') {
      console.error('买入股票失败:', error);
    }
  }
};

// 卖出股票
const sellStock = async () => {
  if (!currentStock.value) {
    ElMessage.warning('请先选择股票');
    return;
  }
  
  try {
    await ElMessageBox.confirm(
      `确定要卖出 ${tradeAmount.value}股 ${currentStockInfo.value.name} 吗？`,
      '卖出确认',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning',
      }
    );
    
    await sellStockApi({
      StockId: currentStock.value,
      Quantity: tradeAmount.value
    });
    
    ElMessage.success(`已成功卖出 ${tradeAmount.value}股 ${currentStockInfo.value.name}`);
    
    // 刷新数据
    await Promise.all([
      fetchTradeHistory(),
      fetchPortfolioList(),
      loadUserInfo() // 卖出后更新用户钱钱
    ]);
  } catch (error) {
    if (error !== 'cancel') {
      console.error('卖出股票失败:', error);
    }
  }
};

// 新闻详情弹窗
const newsDialogVisible = ref(false);
const currentNewsDetail = ref({
  title: '',
  content: '',
  publishTime: '',
  source: ''
});

// 显示新闻详情
const showNewsDetail = (news) => {
  currentNewsDetail.value = news;
  newsDialogVisible.value = true;
};

// 生命周期钩子
onMounted(async () => {
  // 先获取股市列表
  await fetchStockMarkets();

  // 初始化数据，从API获取
  await Promise.all([
    fetchNewsList(),
    fetchPortfolioList(),
    fetchTradeHistory()
  ]);

  // 加载用户信息
  await loadUserInfo();
});

// 刷新股票图表
const refreshStockChart = async () => {
  await fetchStockPriceRecords();
  ElMessage.success('股票数据已刷新');
};
</script>

<style scoped>
.stock-dashboard {
  background-color: #0d1117;
  color: #e6edf3;
  padding: 20px;
  border-radius: 8px;
  display: flex;
  flex-direction: column;
  gap: 20px;
  min-height: 100vh;
  width: 100%;
  box-sizing: border-box;
  font-family: 'PingFang SC', 'Microsoft YaHei', sans-serif;
}

.stock-selector {
  display: none; /* 移除原来的选择器区域 */
}

.stock-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 10px 20px;
  background-color: #161b22;
  border-radius: 8px;
  box-shadow: 0 0 10px rgba(0, 255, 255, 0.1);
  border: 1px solid #30363d;
}

.title-area {
  flex: 1;
}

.selector-area {
  display: flex;
  align-items: center;
  gap: 10px;
}

.title {
  margin: 0;
  color: #58a6ff;
  text-shadow: 0 0 5px rgba(88, 166, 255, 0.3);
  white-space: nowrap;
  display: flex;
  align-items: center;
}

.title-desc {
  font-size: 0.6em;
  color: #8b949e;
  font-style: italic;
  margin-left: 10px;
  font-weight: normal;
}

.main-content {
  display: grid;
  grid-template-columns: 1fr 2fr 1fr;
  gap: 20px;
  height: 60vh;
  min-height: 400px;
}

.news-section, .stock-panel, .trade-history {
  background-color: #161b22;
  border-radius: 8px;
  padding: 15px;
  overflow: hidden;
  box-shadow: 0 0 10px rgba(0, 255, 255, 0.1);
  border: 1px solid #30363d;
  display: flex;
  flex-direction: column;
}

.section-header {
  display: flex;
  align-items: center;
  margin-bottom: 15px;
  padding-bottom: 10px;
  border-bottom: 1px solid #30363d;
}

.section-header h3 {
  margin: 0;
  margin-left: 10px;
  color: #58a6ff;
}

.icon {
  color: #58a6ff;
}

.news-list {
  overflow-y: auto;
  flex: 1;
}

.news-item {
  padding: 12px;
  border-bottom: 1px solid #30363d;
  transition: all 0.2s;
  cursor: pointer;
}

.news-item:hover {
  background-color: #1c2128;
  transform: translateX(3px);
}

.news-date {
  font-size: 0.8em;
  color: #8b949e;
  margin-bottom: 5px;
}

.news-title {
  font-size: 0.9em;
  line-height: 1.4;
  margin-top: 5px;
}

.stock-info {
  text-align: center;
  margin-bottom: 10px;
  padding: 5px;
}

.stock-info h3 {
  margin: 0 0 5px 0;
  font-size: 1.2em;
}

.price-info {
  display: flex;
  flex-direction: row;
  align-items: center;
  justify-content: center;
  gap: 10px;
}

.current-price {
  font-size: 1.6em;
  font-weight: bold;
}

.price-up {
  color: #3fb950;
}

.price-down {
  color: #f85149;
}

.stock-chart {
  flex: 1;
  margin: 10px 0;
  height: 320px;
}

.trade-actions {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 15px;
  margin-top: 10px;
}

.circle-button {
  width: 60px;
  height: 60px;
  font-size: 14px;
  display: flex;
  justify-content: center;
  align-items: center;
}

.history-list {
  overflow-y: auto;
  flex: 1;
}

.history-item {
  padding: 12px;
  border-bottom: 1px solid #30363d;
  display: flex;
  flex-direction: column;
}

.history-header {
  display: flex;
  justify-content: space-between;
  margin-bottom: 8px;
}

.stock-name {
  font-weight: bold;
  color: #e6edf3;
}

.time {
  font-size: 0.8em;
  color: #8b949e;
}

.history-details {
  display: flex;
  align-items: center;
  gap: 10px;
}

.buy-type {
  color: #3fb950;
  font-weight: bold;
}

.sell-type {
  color: #f85149;
  font-weight: bold;
}

.total {
  margin-left: auto;
  font-weight: bold;
}

.portfolio {
  background-color: #161b22;
  border-radius: 8px;
  padding: 15px;
  box-shadow: 0 0 10px rgba(0, 255, 255, 0.1);
  border: 1px solid #30363d;
  height: 1000px;
}

.total-value {
  margin-left: auto;
  font-weight: bold;
  color: #3fb950;
}

/* 修改Element Plus的默认样式以匹配主题 */
:deep(.el-select),
:deep(.el-input),
:deep(.el-button),
:deep(.el-table) {
  --el-bg-color: #161b22;
  --el-text-color-primary: #e6edf3;
  --el-border-color: #30363d;
}

:deep(.el-table) {
  background-color: transparent;
}

:deep(.el-table th) {
  background-color: #1c2128;
}

:deep(.el-table tr) {
  background-color: transparent;
}

:deep(.el-table--striped .el-table__body tr.el-table__row--striped td) {
  background-color: #1c2128;
}

:deep(.el-table td, .el-table th) {
  border-color: #30363d;
}

:deep(.el-select) {
  width: 120px;
}

:deep(.el-input-number) {
  width: 120px;
}

/* 调整表格最大高度 */
:deep(.el-table__body-wrapper) {
  overflow-y: auto;
  max-height: calc(35vh - 80px);
}

.news-source {
  font-size: 0.75em;
  color: #58a6ff;
  margin-left: 5px;
}

.loading-news, .empty-news {
  padding: 15px;
  text-align: center;
  color: #8b949e;
}

.news-more {
  text-align: center;
  padding: 10px 0;
}

.loading-portfolio {
  padding: 20px;
  margin-top: 10px;
}

.el-empty {
  margin: 10px 0;
}

.loading-history, .empty-history {
  padding: 15px;
  text-align: center;
  color: #8b949e;
}

.loading-chart {
  height: 300px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.stock-code {
  float: right;
  color: #8b949e;
  font-size: 0.8em;
}

.selector-area :deep(.el-select) {  
  width: 200px;
}

/* 修改骨架屏颜色，使其适合深色主题 */
.stock-dashboard  :deep(.el-skeleton__item) {
  background-color: #30363d !important;
}

.stock-dashboard  :deep(.el-skeleton__paragraph) {
  height: auto;
}

.stock-dashboard  :deep(.el-skeleton__p) {
  background-color: #30363d !important;
}

.stock-dashboard  :deep(.el-skeleton__first-line), 
.stock-dashboard  :deep(.el-skeleton__paragraph > li) {
  background-color: #30363d !important;
  height: 16px !important;
  margin-top: 12px !important;
}

/* 骨架屏动画效果调整 */
.stock-dashboard  :deep(.is-animated .el-skeleton__item) {
  background: linear-gradient(90deg, #30363d 25%, #383f47 37%, #30363d 63%) !important;
  background-size: 400% 100% !important;
}

.stock-chart .el-empty{
  margin: 0;
  padding: 0;
}

.stock-dashboard :deep(.el-empty) {
  height: auto;
  min-height: 60px;
}

:deep(.stock-dashboard .el-empty__image) {
  width: 100px;
  height: 100px;
}

:deep(.stock-dashboard .el-empty__description) {
  margin-top: 5px;
  font-size: 12px;
}

/* 新闻详情弹窗样式 */
:deep(.news-detail-dialog .el-dialog__header) {
  padding: 15px 20px;
}

:deep( .news-detail-dialog .el-dialog__title) {
  font-size: 18px;
  font-weight: bold;
  color: #ffffff !important;
  text-shadow: 0 0 3px rgba(255, 255, 255, 0.3);
}

:deep(.news-detail-dialog .el-dialog__body) {
  padding: 20px;
  color: #e6edf3;
}

.news-detail-header {
  display: flex;
  justify-content: space-between;
  margin-bottom: 15px;
  padding-bottom: 10px;
  border-bottom: 1px solid #30363d;
  color: #8b949e;
  font-size: 0.9em;
}

.news-detail-dialog .news-detail-content {
  line-height: 1.6;
  white-space: pre-line;
  font-size: large;
}

/* 修改Element弹窗适应深色主题 */
.stock-dashboard :deep(.el-dialog) {
  background-color: #161b22;
  border: 1px solid #30363d;
  border-radius: 8px;
  box-shadow: 0 0 15px rgba(0, 0, 0, 0.5);
}

.stock-dashboard :deep(.el-dialog__headerbtn .el-dialog__close) {
  color: #8b949e;
}

.stock-dashboard :deep(.el-dialog__headerbtn:hover .el-dialog__close) {
  color: #58a6ff;
}

.return-button {
  margin-left: 10px;
  background-color: #58a6ff;
  border-color: #58a6ff;
  transition: all 0.3s;
}

.return-button:hover {
  background-color: #388bfd;
  border-color: #388bfd;
  transform: translateY(-2px);
  box-shadow: 0 2px 5px rgba(88, 166, 255, 0.4);
}

.user-money {
  color: #e6edf3;
  font-size: 14px;
  margin-right: 15px;
  padding: 4px 8px;
  background-color: #21262d;
  border-radius: 4px;
  border: 1px solid #30363d;
}

.money-value {
  font-weight: bold;
  color: #7ee787;
  margin-left: 4px;
}

.chart-header {
  position: relative;
  height: 0;
}

.refresh-button {
  position: absolute;
  top: 10px;
  right: 10px;
  z-index: 2;
  background-color: #21262d;
  border-color: #30363d;
  transition: all 0.3s;
}

.refresh-button:hover {
  background-color: #58a6ff;
  border-color: #58a6ff;
  transform: rotate(180deg);
}
</style>
