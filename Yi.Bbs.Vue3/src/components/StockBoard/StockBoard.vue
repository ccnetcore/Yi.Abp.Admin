<template>
  <div class="stock-board">
    <h2>股票看板</h2>
    <div class="stock-list">
      <div v-for="stock in stocks" :key="stock.code" class="stock-item">
        <div class="stock-info">
          <h3>{{ stock.name }} ({{ stock.code }})</h3>
          <p :class="getPriceClass(stock)">¥{{ stock.currentPrice }}</p>
          <p>涨跌幅: <span :class="getPriceClass(stock)">{{ stock.changePercent }}%</span></p>
        </div>
        <div class="stock-actions">
          <button @click="handleBuy(stock)">买入</button>
          <button @click="handleSell(stock)">卖出</button>
        </div>
      </div>
    </div>
    
    <!-- 交易弹窗 -->
    <div v-if="showTradeModal" class="trade-modal">
      <h3>{{ tradeType === 'buy' ? '买入' : '卖出' }} {{ selectedStock.name }}</h3>
      <p>当前价格: ¥{{ selectedStock.currentPrice }}</p>
      <div class="form-group">
        <label>数量:</label>
        <input v-model="tradeAmount" type="number" min="1" />
      </div>
      <div class="form-group">
        <label>总金额: ¥{{ totalAmount }}</label>
      </div>
      <div class="modal-actions">
        <button @click="confirmTrade">确认</button>
        <button @click="cancelTrade">取消</button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
import { useStocks } from '@/composables/useStocks'
import { useTrades } from '@/composables/useTrades'

const { stocks } = useStocks()
const { executeTrade } = useTrades()

const showTradeModal = ref(false)
const selectedStock = ref({})
const tradeType = ref('')
const tradeAmount = ref(1)

const totalAmount = computed(() => {
  return (selectedStock.value.currentPrice * tradeAmount.value).toFixed(2)
})

function getPriceClass(stock) {
  return {
    'price-up': stock.changePercent > 0,
    'price-down': stock.changePercent < 0,
    'price-unchanged': stock.changePercent === 0
  }
}

function handleBuy(stock) {
  selectedStock.value = stock
  tradeType.value = 'buy'
  showTradeModal.value = true
}

function handleSell(stock) {
  selectedStock.value = stock
  tradeType.value = 'sell'
  showTradeModal.value = true
}

function confirmTrade() {
  executeTrade({
    stockCode: selectedStock.value.code,
    stockName: selectedStock.value.name,
    price: selectedStock.value.currentPrice,
    amount: tradeAmount.value,
    type: tradeType.value,
    date: new Date().toISOString()
  })
  
  cancelTrade()
}

function cancelTrade() {
  showTradeModal.value = false
  tradeAmount.value = 1
}
</script>

<style scoped>
.stock-board {
  padding: 1rem;
}

.stock-list {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 1rem;
}

.stock-item {
  border: 1px solid #eee;
  border-radius: 8px;
  padding: 1rem;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.price-up {
  color: #ff4d4f;
}

.price-down {
  color: #52c41a;
}

.price-unchanged {
  color: #666;
}

.trade-modal {
  position: fixed;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  background: white;
  padding: 2rem;
  border-radius: 8px;
  box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
  z-index: 1000;
}

.form-group {
  margin: 1rem 0;
}

.modal-actions {
  display: flex;
  justify-content: flex-end;
  gap: 1rem;
  margin-top: 1rem;
}
</style> 