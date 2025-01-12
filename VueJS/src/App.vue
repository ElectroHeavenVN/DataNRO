<script setup>
import Item from './components/Item.vue'

const server ='Server8910';

</script>

<template>
  <Item
    width="450px"
    :icon=28420
    name="Cải Trang Siêu Xayda Đô Re mon"
    description="Doremon đã dùng tàu xuyên thời gian để về gặp sư phụ quy lão, nhờ sự chỉ dạy của quy lão doremon đã thành 1 siêu xayda chính hiệu"
    :id=2422
    type="Cải trang"
    gender="Chung"
    :level=1
    powerRequired="100M"
    :isNewItem="true"
    class="hoverable"
  />
</template>

<style>
.hoverable:hover {
    transform: translateY(-3px);
    box-shadow: 0 3px 10px #0003;
}

.hoverable {
    transition-duration: .2s;
}
</style>
