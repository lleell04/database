 <!-- 这里是商品管理界面的内容区域 -->
<template>
  <div class="CommodityShow">
    <!-- 搜索和筛选按钮 --> 
    <div class="SearchContainer">
      <el-input v-model="searchName" placeholder="请输入商品名称(在全部商品中搜索)" style="display: inline-block;"
         @keyup.enter="filterProducts"></el-input>
      <el-button type="primary" @click="filterProducts">搜索</el-button>
      <el-input v-model="searchcategoryInit" placeholder="请输入商家分类（在全部商品中搜索）" style="display: inline-block;"
         @keyup.enter="filterProductsTag"></el-input>
      <el-button type="primary" @click="filterProductsTag">筛选</el-button>
    </div>
    <!-- 表格 -->
    <div class="TableContainer">
      <el-table :data="currentPageData" class="CommodityTable" height="760"@selection-change="handleSelectionChange">
        <el-table-column type="selection" />
        <el-table-column prop="id" label="商品ID"></el-table-column>
        <el-table-column prop="name" label="商品名称" ></el-table-column>
        <el-table-column prop="categoryInit" label="商家分类" ></el-table-column>
        <el-table-column prop="price" label="商品价格" width="100"></el-table-column>
        <el-table-column prop="isOnSale" label="是否出售" width="100">
          <template #default="scope">
            <el-tag type="success" v-if="scope.row.isOnSale">是</el-tag>
            <el-tag type="info" v-else>否</el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作">
          <template #default="scope">
            <el-button-group>
              <el-button size='small' type="primary" icon="check" @click="handleCheck(scope.row)">查看</el-button>
              <el-button size='small' type="primary" icon="Edit" @click="handleEdit(scope.row)">编辑</el-button>
              <el-button size='small' type="danger" icon="Delete" @click="handleDelete(scope.row)">删除</el-button>
            </el-button-group>
          </template>
        </el-table-column>
      </el-table>
    </div>  

    <!-- 编辑 -->
    <div v-if="dialogVisibleTwo" class="SettingPopUp">
      <div v-if="preProduct" class="SettingContent" >
        <el-form ref="editForm" :rules="rules" :model="preProduct">
          <el-form-item label="商品名称" prop="name">
            <el-input v-model="preProduct.name"></el-input>
          </el-form-item>
          <el-form-item label="商品价格" prop="price">
            <el-input v-model.number="preProduct.price"></el-input>
          </el-form-item>
          <el-form-item label="系统分类" prop="categorySys" >
              <el-select v-model="preProduct.categorySys" @change="updateTagT">
                <el-option-group
                        v-for="group in categories"
                        :key="group.largeCategoryName"
                        :label="group.largeCategoryName"
                      >
                        <el-option
                          v-for="item in group.subCategories"
                          :key="item.subCategoryId"
                          :label="item.subCategoryName"
                          :value="item.subCategoryId"
                        />
                </el-option-group>
              </el-select>
          </el-form-item>
          <el-form-item label="商家分类" prop="categoryInit">
            <el-input v-model.number="preProduct.categoryInit"></el-input>
          </el-form-item>
          <el-form-item label="商品具体描述" prop="description">
            <el-input v-model="preProduct.description"></el-input>
          </el-form-item>
          <el-form-item label="商品图片">
            <!-- 显示已存在的图片 -->
            <div v-if="preProduct.images && preProduct.images.length" style="display: flex; flex-wrap: wrap;">
              <div v-for="(image, index) in preProduct.images" :key="image.imageId" style="position: relative; margin-right: 10px;">
                <img :src="image.imageUrl" alt="商品图片" style="margin-top:10px;width: 100px; height: 100px; object-fit: cover;border-radius: 8px;" />
                <span class="close" style="position: absolute; margin-top:10px; top: 0; right: 5px; color: #82111f;" @click="removeImage(index)">&times;</span>
              </div>
            </div>
            <!-- 显示选择的图片 -->
            <div v-if="selectedImages.length" style="display: flex; flex-wrap: wrap;">
              <div v-for="(image, index) in selectedImages" :key="index" style="position: relative; margin-right: 10px;">
                <img :src="image.url" alt="选择的图片" style="margin-top:10px;width: 100px; height: 100px; object-fit: cover;border-radius: 8px;" />
                <span class="close" style="position: absolute; margin-top:10px; top: 0; right: 5px; color: #82111f;" @click="removeSelectedImage(index)">&times;</span>
              </div>
            </div>
            <!-- 上传新图片 -->
            <el-form-item label="">
              <input type="file" @change="handleFile" multiple />
            </el-form-item>
          </el-form-item>
          <el-form-item label="商品图片及描述">
          <!-- 显示已存在的瑕疵图片和描述 -->
          <div v-if="preProduct.defectImages && preProduct.defectImages.length" style="display: flex; flex-wrap: wrap;">
            <div v-for="(defect, index) in preProduct.defectImages" :key="defect.imageId" style="position: relative; margin-right: 10px; margin-bottom: 20px; text-align: center;">
              <!-- 图片显示 -->
              <img :src="defect.imageUrl" alt="瑕疵图片" style="width: 50px; height: 50px; object-fit: cover; border-radius: 8px;" />
              <!-- 输入框，显示在图片底部 -->
              <el-input   @input="limitInputLength" v-model="defect.description" placeholder="输入描述" style="width: 100px; margin-top: 5px; display: block; margin: 5px auto;"maxlength="150" show-word-limit></el-input>
              <!-- 叉号，显示在图片右上角 -->
              <span class="close" style="position: absolute; top: 2px; right: 2px; color: #82111f; cursor: pointer;" @click="removeDefectImage(index)">&times;</span>
            </div>
          </div>

          <!-- 显示选择的瑕疵图片 -->
          <div v-if="selectedDefectImages.length" style="display: flex; flex-wrap: wrap;">
            <div v-for="(image, index) in selectedDefectImages" :key="index" style="position: relative; margin-right: 10px; margin-bottom: 20px; text-align: center;">
              <!-- 图片显示 -->
              <img :src="image.url" alt="选择的瑕疵图片" style="width: 50px; height: 50px; object-fit: cover; border-radius: 8px;" />
              <!-- 输入框，显示在图片底部 -->
              <el-input   @input="limitInputLength" v-model="image.description" placeholder="输入图片描述" style="width: 100px; margin-top: 5px; display: block; margin: 5px auto;"maxlength="150" show-word-limit></el-input>
              <!-- 叉号，显示在图片右上角 -->
              <span class="close" style="position: absolute; top: 2px; right: 2px; color: #82111f; cursor: pointer;" @click="removeSelectedDefectImage(index)">&times;</span>
            </div>
          </div>
            
            <!-- 上传瑕疵图片 -->
            <el-form-item label="">
              <input type="file" @change="handleDefectFile" multiple />
            </el-form-item>
            </el-form-item>

          <div style="display: flex;justify-content: center;margin-top: 30px">
            <el-button type="primary" @click="onsubmit()">保存</el-button>
            <el-button  type="primary"  @click="handleCancel">取消</el-button>
          </div>
        </el-form>
       </div>
    </div>   

    <!--添加商品对话框-->
    <div v-if="addDialogVisible" class="SettingPopUp">
      <div class="SettingContent">
        <el-form :model="newProduct" :rules="rules" ref="form">
            <el-form-item label="商品名称" prop="name">
              <el-input v-model="newProduct.name"></el-input>
            </el-form-item>
            <el-form-item label="系统分类" prop="categorySys">
              <el-select v-model="newProduct.categorySys" @change="updateTag">
                  <el-option-group
                    v-for="group in categories"
                    :key="group.largeCategoryName"
                    :label="group.largeCategoryName"
                  >
                    <el-option
                      v-for="item in group.subCategories"
                      :key="item.subCategoryId"
                      :label="item.subCategoryName"
                      :value="item.subCategoryId"
                    />
                  </el-option-group>
              </el-select>
            </el-form-item>
            <el-form-item label="商家分类" prop="categoryInit">
              <el-input v-model="newProduct.categoryInit"></el-input>
            </el-form-item>
            <el-form-item label="商品价格" prop="price">
              <el-input v-model.number="newProduct.price"></el-input>
            </el-form-item>
            <el-form-item label="商品描述" prop="description">
              <el-input v-model="newProduct.description"></el-input>
            </el-form-item>
            <!-- 商品图片展示 -->
            <el-form-item label="商品图片" prop="images">
              <div v-if="newProduct.images.length > 0" class="image-preview">
                <div v-for="(image, index) in newProduct.images" :key="index" class="image-item">
                  <img
                    :src="generateImageURL(image)"
                    alt="图片"
                    style="width: 100px; height: 100px; object-fit: cover; margin-right: 5px;"
                  />
                </div>
              </div>
              <input type="file" @change="onFileChange" accept="image/*" />
            </el-form-item>
            <!-- 瑕疵图文描述展示 -->
            <el-form-item label="商品图文描述（图文一一对应）" prop="imagesWithText">
              <div v-for="(item, index) in newProduct.imagesWithText" :key="index" class="image-text-item" style="display: flex; flex-direction: column; align-items: center; margin-bottom: 10px; margin-right: 10px;">
                <img
                  :src="generateImageURL(item.image)"
                  alt="瑕疵图片"
                  style="width: 50px; height: 50px; object-fit: cover; margin-bottom: 5px;"
                />
                <p>{{ item.text }}</p>
              </div>
            </el-form-item>
            <!-- 瑕疵图文添加 -->
            <el-form-item>
              <div style="display: flex; align-items: center; margin-bottom: 10px;">
                <input type="file" @change="handleFileChange" id="defect-image-input" accept="image/*" />
                <el-input
                  type="textarea"
                  :rows="3"
                  v-model="newImageText"
                  placeholder="输入文字描述"
                  style="margin-left: 10px; flex: 1;"
                ></el-input>
                <el-button type="primary" @click="addImageWithText" style="margin-left: 10px;">添加</el-button>
              </div>
            </el-form-item>
        </el-form>

        <div style="margin-top: 20px; display: flex; justify-content: flex-end;">
          <el-button type="primary" @click="addNewProduct">添加</el-button>
          <el-button type="default" @click="() => addDialogVisible = false" style="margin-left: 10px;">取消</el-button>
        </div>
      </div>
    </div>

    <!-- 添加商品和批量删除按钮 -->
    <div id="BottomButton">
      <el-button size='small' type='primary' icon="Edit" @click="showAddDialog()">添加商品</el-button>
      <el-button size='small' type='danger' icon="Delete" @click="confirmBatchDelete()">批量删除</el-button>
    </div>

    <!-- 确认批量删除对话框 -->
    <div v-if="confirmDialogVisible" class="SettingPopUp">
        <div class="SettingContent">
          <span>您确定要删除选中的商品吗？此操作不可撤销。</span>
          <el-button type="primary" @click="deleteSelectedCommodities">确认</el-button>
          <el-button @click="confirmDialogVisible = false">取消</el-button>
        </div>
    </div>   

    <!-- 翻页 -->
    <div class="paginationContainer">
      <el-pagination
        v-model:current-page="currentPage"
        :page-size="pageSize"
        :total="totalProducts"
        layout="total, prev, pager, next, jumper"
        @current-change="handlePageChange">
      </el-pagination>
    </div>
    
  </div>
</template>

<script >
import { ref, computed ,onMounted,reactive } from 'vue';
import axiosInstance from '../components/axios';
import { useRouter } from 'vue-router';

import { ElSelect, ElOption,ElMessageBox } from 'element-plus';
import { ElTable, ElTableColumn, ElButton, ElDialog, ElPagination,ElMessage  } from 'element-plus';

import 'element-plus/dist/index.css';

export default{
  components: {
    ElSelect,
    ElOption,
    ElTable, 
    ElTableColumn,
    ElButton, 
    ElDialog,
    ElPagination ,
  },
  setup() {
    const searchName=ref('');
    const searchcategoryInit=ref('');
    const value = ref('');
    const dialogVisible = ref(false);
    const dialogVisibleTwo = ref(false);
    const preProduct = ref({});
    const currentProduct = ref({});
    const editForm = ref(null);
    const products=ref([]);//记录商品数据
    const pageSize = 20;
    const currentPage = ref(1);
    const totalProducts = ref(0);//记录商品总数
    const addDialogVisible = ref(false);
    const form = ref(null);
    const confirmDialogVisible = ref(false);
    const selectedProducts = ref([]);
    const viewType = ref(1);
    const router = useRouter();
    const selectedImages = ref([]); 
    const deletedImages= ref([]); 
    const selectedDefectImages = ref([]);
    const deletedDefectImages= ref([]);  
    const categories = reactive([]);
    const message01 = ref('');

//根据商品是否售出展示商品
const fetchProducts = async () => {
  const storeId = localStorage.getItem('userId'); 

  try {
  const response = await axiosInstance.get('/StoreViewProduct/GetProductsByStoreIdAndViewType', {
    params: {
      StoreId: storeId,
      ViewType: viewType.value
    },
  });
  // console.log('Backend Response:', response.data);

  if (Array.isArray(response.data)) {
    const processedProducts = response.data.map(product => {
      // console.log('Original Product Data:', product);

      return {
        id: product.productId || 'N/A',
        name: product.productName || 'Unknown',
        categoryInit: product.storeTag || 'Unknown',
        categorySys: product.subTag || 'Unknown',
        price: product.productPrice || 0,
        isOnSale: product.saleOrNot !== undefined ? product.saleOrNot : false,
        description: product.description || 'No description available',
      };
    });
    console.log('Processed Products:', processedProducts);

    products.value = processedProducts; 
    totalProducts.value = processedProducts.length;
  } else {
    console.error('Unexpected response format:', response.data);
  }} catch (error) {
    console.error('获取商品数据失败:', error);
  }
};
const handleChange = (viewTypeValue) => {
      viewType.value = viewTypeValue;
      fetchProducts();
};
//根据商品名称搜索商品
const filterProducts = () => {
      if (searchName.value.trim() !== '') {
        fetchProductByName(searchName.value.trim());
      } else {
        fetchProducts();
      }
};
const fetchProductByName = async (keyword) => {
  const storeId = localStorage.getItem('userId'); 
  try {
    const response = await axiosInstance.get('/StoreViewProduct/search', {
      params: {
        storeId: storeId,
        keyword: keyword || '',
      }
    });

    if (Array.isArray(response.data)) {
      const processedProducts = response.data.map(product => {

        return {
        id: product.productId || 'N/A',
        name: product.productName || 'Unknown',
        categorySys: product.subTag || 'Unknown',
        categoryInit: product.storeTag || 'Unknown',
        price: product.productPrice || 0,
        isOnSale: product.saleOrNot !== undefined ? product.saleOrNot : false,
        description: product.description || 'No description available',
        };
      });

      products.value = processedProducts; 
    } else {
      console.error('Unexpected response format:', response.data);
    }
  } catch (error) {
    products.value = [];
    console.error('通过商品名称获取商品数据失败:', error);
  }
};
//根据商品分类搜索商品
const filterProductsTag = () => {
      if (searchcategoryInit.value.trim() !== '') {
        fetchProductByTag(searchcategoryInit.value.trim());
      } else {
        fetchProducts();
      }
};
const fetchProductByTag = async (storeTag) => {
  const storeId = localStorage.getItem('userId'); 
  try {
    const response = await axiosInstance.get('/StoreViewProduct/searchByStoreTag', {
      params: {
        storeId: storeId,
        storeTag: storeTag || '',
      }
    });

    if (Array.isArray(response.data)) {
      const processedProducts = response.data.map(product => {

        return {
        id: product.productId || 'N/A',
        name: product.productName || 'Unknown',
        categorySys: product.subTag || 'Unknown',
        categoryInit: product.storeTag || 'Unknown',
        price: product.productPrice || 0,
        isOnSale: product.saleOrNot !== undefined ? product.saleOrNot : false,
        description: product.description || 'No description available',
        };
      });

      products.value = processedProducts; 
    } else {
      console.error('Unexpected response format:', response.data);
    }
  } catch (error) {
    products.value = [];
    console.error('通过商品名称获取商品数据失败:', error);
  }
};
//获取商品系统分类并过滤
const fetchCategories = async () => {
  try {
    const response = await axiosInstance.get('/Classification/GetAllCategories');

    // 要过滤掉的subCategoryId列表
    const idsToRemove = ['00000', '01000', '02000', '03000', '04000'];
    
    // 处理过滤操作
    const filteredCategories = response.data.categories.map(category => {
      return {
        ...category,
        subCategories: category.subCategories.filter(subCategory => !idsToRemove.includes(subCategory.subCategoryId))
      };
    });
    
    // 更新 categories
    categories.splice(0, categories.length, ...filteredCategories);

     // 调试输出
     console.log('Updated categories:', categories);
    
    message01.value = '已获取系统分类数据';
    // console.log(categories);
  } catch (error) {
    if (error.response) {
      message01.value = error.response.data;
    } else {
      message01.value = '获取分类数据失败';
    }
    ElMessage.error('获取分类数据失败，请稍后再试');
  }
};
onMounted(async () => {
    await fetchCategories(); // 确保分类数据已加载
    await fetchProducts(); // 分类数据加载完成后获取商品数据
});
//查看商品具体信息
const handleCheck = (item) => {
    currentProduct.value = item;
    localStorage.setItem('productIdOfDetail', item.id);
    router.push('/productdetail');
};
function calculateByteLength(str) {
  let length = 0;
  for (let i = 0; i < str.length; i++) {
    const char = str.charAt(i);
    if (/[\u0000-\u007F]/.test(char)) {
      // 英文字符或数字，1字节
      length += 1;
    } else {
      // 中文字符及其他，3字节
      length += 3;
    }
  }
  return length;
}
const rules = {
      // name: [{ required: true, message: '请输入商品名称', trigger: 'blur' }],
      name: [
          { required: true, message: '请输入商品名称', trigger: 'blur' },
          { 
            validator: (rule, value, callback) => {
              if (!value) {
                callback(new Error('请输入商品名称'));
              } else if (calculateByteLength(value) > 40) {
                callback(new Error('商品名称过长'));
              } else {
                callback();
              }
            }, 
            trigger: 'blur'
          }
        ],
      categorySys: [{ required: true, message: '请输入系统分类', trigger: 'blur' }],
      // categoryInit: [{ required: true, message: '请输入商家分类', trigger: 'blur' }],
      categoryInit: [
          { required: true, message: '请输入商家分类', trigger: 'blur' },
          { 
            validator: (rule, value, callback) => {
              if (!value) {
                callback(new Error('请输入商家分类'));
              } else if (calculateByteLength(value) > 40) {
                callback(new Error('商家分类过长'));
              } else {
                callback();
              }
            }, 
            trigger: 'blur'
          }
        ],
      price: [{ required: true, message: '请输入商品价格', trigger: 'blur' } ,
      { type: 'number', message: '价格必须为数字', trigger: 'blur' },
      { validator: (rule, value, callback) => {
            if (value === null || value === undefined || value < 0) {
              callback(new Error('价格必须大于或等于0'));
            } else {
              callback();
            }
          }, trigger: 'blur' }],
      isOnSale: [{ required: true, message: '请选择是否出售', trigger: 'change' }],
      // description: [{ required: true, message: '请输入商品描述', trigger: 'blur' }],
      description: [
          { required: true, message: '请输入商品描述', trigger: 'blur' },
          { 
            validator: (rule, value, callback) => {
              if (!value) {
                callback(new Error('请输入商品描述'));
              } else if (calculateByteLength(value) > 190) {
                callback(new Error('商品描述过长'));
              } else {
                callback();
              }
            }, 
            trigger: 'blur'
          }
        ],
      images: [
    { validator: (rule, value, callback) => {
      if (value.length === 0) {
        callback(new Error('请上传至少一张商品图片'));
      } else {
        callback();
      }
    }, trigger: 'change' }
  ],
  //     imagesWithText: [
  //   { validator: (rule, value, callback) => {
  //       if (!Array.isArray(value) || value.length === 0) {
  //         callback(new Error('请添加至少一组图文描述'));
  //       } else {
  //         callback();
  //       }
  //     }, trigger: 'change'
  //   }
  // ]
  imagesWithText: [
          { 
            validator: (rule, value, callback) => {
              if (!Array.isArray(value) || value.length === 0) {
                callback(new Error('请添加至少一组图文描述'));
              } else {
                let isValid = true;
                for (let i = 0; i < value.length; i++) {
                  const item = value[i];
                  if (calculateByteLength(item.text) > 490) {
                    callback(new Error(`图文描述的文本内容过长`));
                    isValid = false;
                    break;
                  }
                }
                if (isValid) {
                  callback();
                }
              }
            },
            trigger: 'change'
          }
        ]
};
//编辑商品
// 限制输入字符长度的方法
const limitInputLength = (event) => {
  const maxLength = 150;
  const input = event.target.value;

  // 如果输入长度超过最大限制，截取前 maxLength 个字符
  if (input.length > maxLength) {
    description.value = input.slice(0, maxLength);
  }
};
//移除新选中的商品图片
const removeSelectedImage = (index) => {
  selectedImages.value.splice(index, 1);
};
//编辑中删除服务器中的商品图片
const removeImage = (index) => {
    if (preProduct.value.images.length > 1) {
    // 从显示的图片数组中移除
    const removedImage = preProduct.value.images.splice(index, 1)[0]; // 从 preProduct.images 中移除图片，并获取该图片对象

    // 将已删除的图片对象添加到 deletedImages 数组中
    if (removedImage) {
      deletedImages.value.push(removedImage);
    }
  } else {
    ElMessage({
      message: '至少需要保留一张商品图片',
      type: 'warning'
    });
  }
};
//移除新选中的瑕疵图文
const removeSelectedDefectImage = (index) => {
  selectedDefectImages.value.splice(index, 1);
};
//编辑中删除服务器中的瑕疵图片
  const removeDefectImage = (index) => {
  // 确保至少保留一张瑕疵图片
  if (preProduct.value.defectImages.length > 1) {
  // 从显示的瑕疵图片数组中移除
  const removedImage = preProduct.value.defectImages.splice(index, 1)[0];
    
  // 将已删除的瑕疵图片对象添加到 deletedDefectImages 数组中
  if (removedImage) {
      deletedDefectImages.value.push(removedImage);
    }
  } else {
    ElMessage({
      message: '至少需要保留一张详细图片',
      type: 'warning'
    });
  }
};
//取消编辑
const handleCancel = () => {
  // 清空选择的商品图片
  selectedImages.value = [];
  // 清空 deletedImages 数组
  deletedImages.value = [];

  // 清空 deletedDefectImages 数组
  deletedDefectImages.value= [];
  // 关闭对话框
  dialogVisibleTwo.value = false;
};
//处理商品图片
const handleFile = (event) => {
      const files = event.target.files;
  if (files.length > 0) {
    const newImages = Array.from(files).map(file => ({
      url: URL.createObjectURL(file),
      file: file,
    }));
    selectedImages.value = [...selectedImages.value, ...newImages];
  }
    };
//处理瑕疵图文
const handleDefectFile = (event) => {
  const files = event.target.files;
  if (files.length > 0) {
    const newImages = Array.from(files).map(file => ({
      url: URL.createObjectURL(file),
      file: file,
       description: '无'
    }));
    selectedDefectImages.value = [...selectedDefectImages.value, ...newImages];
  }
};
//编辑商品框  
const handleEdit = async (item) => {
      if (item.isOnSale) {
        ElMessage({
          message: '该商品已经出售，无法编辑',
          type: 'warning'
        });
        return;
      }
      currentProduct.value = item;
      preProduct.value = {
        id: item.id,
        name: item.name,
        categorySys: item.categorySys,
        categoryInit: item.categoryInit,
        price: item.price,
        description: item.description,
        images: [],
        defectImages:[],

      };

       // 获取商品图片
        try {
          const response = await axiosInstance.get(`/StoreViewProduct/getProductImages/${preProduct.value.id}`);
          if (response.status === 200) {
            preProduct.value.images = response.data;  // 将图片数据赋值给 preProduct.images
          } else {
            ElMessage({
              message: '获取商品图片失败else',
              type: 'error'
            });
          }
        } catch (error) {
          
          ElMessage({
            message: '获取商品图片失败，本商品可能不存在图片',
            type: 'error'
          });
        }
        //获取瑕疵图文
        try {
    const productDetailsResponse = await axiosInstance.get(`/StoreViewProduct/getProductDetails/${preProduct.value.id}`);
    if (productDetailsResponse.status === 200) {
      const productDetails = productDetailsResponse.data;
      console.log('productDetails', productDetails);

      // 确保 data 中的 image 和 description 存在且是数组
      const defectImagesData = productDetails.map(detail => ({
        imageId: detail.image.imageId,
        imageUrl: detail.image.imageUrl,
        description: detail.description || '描述获取失败'
      }));

      // 更新 defectImages
      preProduct.value.defectImages = defectImagesData;

      console.log('Defect Images:', preProduct.value.defectImages);
    } else {
      ElMessage({
        message: '获取详细图片失败',
        type: 'error'
      });
    }
  } catch (error) {
    ElMessage({
      message: '获取详细图片失败，本商品可能不存在详细图片',
      type: 'error'
    });
    console.log('error', error);
  }
        
      dialogVisibleTwo.value = true;
};
//更新瑕疵描述
const updateDefectDescription = async () => {
  try {
    // 遍历 preProduct.defectImages，准备更新描述
    const updateRequests = preProduct.value.defectImages.map(defectImage => {
      return axiosInstance.post('/StoreViewProduct/updateProductDescription', {
        imageId: defectImage.imageId,
        description: defectImage.description || '无' // 如果没有描述，则默认为“无”
      });
    });

    // 等待所有更新请求完成
    const responses = await Promise.all(updateRequests);
    
    // 检查所有响应状态
    const allSuccessful = responses.every(response => response.status === 200);
    if (allSuccessful) {
      ElMessage({
        message: '详细图片文字描述已更新',
        type: 'success'
      });
    } else {
      ElMessage({
        message: '更新详细图片文字描述失败',
        type: 'error'
      });
    }
  } catch (error) {
    console.error('更新详细图片文字描述时发生错误:', error.response ? error.response.data : error.message);
    ElMessage({
      message: '更新详细图片文字描述失败',
      type: 'error'
    });
  }
};
//上传商品图片
const submitUpload = async () => {
  if (selectedImages.value.length > 0) { 
    const formData = new FormData();
    selectedImages.value.forEach(image => {
      formData.append('ProductImages', image.file); // 从 selectedImages.value 中添加文件
    });
    formData.append('ProductId',preProduct.value.id);
    const uploadUrl = `/StoreViewProduct/addProductImage`;
    try {
      const response = await axiosInstance.post(uploadUrl, formData, {
        headers: { 'Content-Type': 'multipart/form-data' }
      });
      if (response.status === 200) {
        ElMessage({
          message: '图片上传成功',
          type: 'success'
        });
        // 清空选择的图片列表
        selectedImages.value = [];
      } else {
        ElMessage({
          message: '图片上传失败1',
          type: 'error'
        });
      }
    } catch (error) {
      console.error('上传图片时发生错误:', error.response ? error.response.data : error.message);
      ElMessage({
        message: '图片上传失败2',
        type: 'error'
      });
    }
  }
};
// 上传瑕疵图文
const submitDefectUpload = async () => {
  if (selectedDefectImages.value.length > 0) {
    try {
      // 遍历每个选中的瑕疵图片，逐个上传
      for (const image of selectedDefectImages.value) {
        const formData = new FormData();
        
        // 添加文件到 FormData
        formData.append('Image', image.file); // 确保 image.file 是 File 对象

        // 添加描述信息到 FormData
        formData.append('Description', image.description || '无');

        // 添加 productId 到 FormData
        formData.append('productId', preProduct.value.id);

        // 上传的接口地址
        const uploadUrl = `/StoreViewProduct/addProductDetail`;

        // 发起上传请求
        const response = await axiosInstance.post(uploadUrl, formData, {
          headers: { 'Content-Type': 'multipart/form-data' }
        });

        // 处理响应
        if (response.status === 200) {
          ElMessage({
            message: `详细图片 "${image.file.name}" 上传成功`,
            type: 'success'
          });
        } else {
          ElMessage({
            message: `详细图片 "${image.file.name}" 上传失败`,
            type: 'error'
          });
        }
      }

      // 清空已上传的图片数组
      selectedDefectImages.value = [];

    } catch (error) {
      console.error('上传详细图片时发生错误:', error.response ? error.response.data : error.message);
      ElMessage({
        message: '详细图片上传失败',
        type: 'error'
      });
    }
  } 
};
//给后端传大分类名称Tag
const updateTagT = (selectedSubCategoryId) => {
  // 查找选中的系统分类对应的大标题名字
  const selectedCategory = categories.find(group =>
    group.subCategories.some(item => item.subCategoryId === selectedSubCategoryId)
  );

  // 更新 Tag 字段
  preProduct.value.Tag = selectedCategory ? selectedCategory.largeCategoryName : '';
};
//提交
const onsubmit = async () => {
  editForm.value.validate(async (valid) => {
    if (valid) {
      const formData = new FormData();
      formData.append('productId', preProduct.value.id);
      formData.append('productName', preProduct.value.name);
      formData.append('productPrice', preProduct.value.price);
      formData.append('storeTag', preProduct.value.categoryInit);
      formData.append('tag', preProduct.value.Tag || ''); 
      formData.append('subTagId', preProduct.value.categorySys || '');
      formData.append('description', preProduct.value.description);
      const userId = localStorage.getItem('userId'); 
      formData.append('storeId', userId);
      for (const [key, value] of formData.entries()) {
    console.log(`${key}: ${value}`);
  }
        try {
          const response = await axiosInstance.put('/StoreViewProduct/editProduct', formData);

          if (response.status === 200) {
            dialogVisibleTwo.value = false;
            currentProduct.value=preProduct.value;
            fetchProducts();
            ElMessage({
              message: '商品信息已更新',
              type: 'success'
            });
          //上传商品图片
          if (selectedImages.value.length > 0) {
            await submitUpload();
          }
          //删除商品图片
          try {
            for (const image of deletedImages.value) {
              const response = await axiosInstance.delete(`/StoreViewProduct/deleteProductImage/${preProduct.value.id}/${image.imageId}`);
              if (response.status === 401) {
                ElMessage({
                  message: '所有图片已被删除',
                  type: 'warning'
                });
              }
            }
          } catch (error) {
            if (error.response && error.response.status === 401) {
            ElMessage({
              message: '所有图片已被删除',
              type: 'warning'
            });
          } else {
            // 处理其他非401错误
            ElMessage({
              message: '删除图片时发生错误',
              type: 'error'
            });
          }
          }
          // 清空 deletedImages 数组
          deletedImages.value = [];
          //上传瑕疵图片
          await submitDefectUpload();
         // 删除瑕疵图片
                  try {
                    for (const defectImage of deletedDefectImages.value) {
                      // 发起删除请求，使用 defectImage.imageId 作为 Path 参数
                      const response = await axiosInstance.post(`/StoreViewProduct/deleteProductDetail/${defectImage.imageId}`);

                      // 处理 401 响应，表示所有瑕疵图片已被删除
                      if (response.status === 401) {
                        ElMessage({
                          message: '所有详细图片已被删除',
                          type: 'warning'
                        });
                      } else {
                        // 如果删除成功，可根据需求处理成功逻辑
                        ElMessage({
                          message: '详细图片已成功删除',
                          type: 'success'
                        });
                      }
                    }
                  } catch (error) {
                    // 针对 401 错误进行处理
                    if (error.response && error.response.status === 401) {
                      ElMessage({
                        message: '所有详细图片已被删除',
                        type: 'warning'
                      });
                    } else {
                      // 处理其他非401错误
                      ElMessage({
                        message: '删除详细图片时发生错误',
                        type: 'error'
                      });
                    }
                  }
                    // 清空 deletedDefectImages 数组
                    deletedDefectImages.value = [];
          //更新瑕疵图文描述
          await updateDefectDescription();
            } else {
                ElMessage({
                    message: '更新商品信息失败?',
                    type: 'error'
                });
            }
            } catch (error) {
                console.error('更新商品信息失败:', error.response ? error.response.data : error.message);
                ElMessage({
                    message: '更新商品信息失败',
                    type: 'error'
                });
            }
        } else {
            ElMessage({
                message: '请填写正确',
                type: 'warning'
            });
        }
    });
};
//翻页
const currentPageData = computed(() => {
const start = (currentPage.value - 1) * pageSize;
const end = Math.min(start + pageSize, totalProducts.value);
  return products.value.slice(start, end);
});
const handlePageChange = (page) => {
  currentPage.value = page;
};
//删除单个商品
const handleDelete = async (row) => {
  if (row.isOnSale) {
    ElMessage({
      message: '该商品已经出售，无法删除',
      type: 'warning'
    });
    return;
  }

  try {
    await ElMessageBox.confirm('此操作将永久删除该商品, 是否继续?', '提示', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    });

    const storeId = localStorage.getItem('userId');; 
    const productId = row.id;

    const response = await axiosInstance.delete('/StoreViewProduct/deleteProducts', {
      params: {
        storeId: storeId
      },
      data: [productId] 
    });

    if (response.status === 200) {
      // 删除本地商品列表中的商品
      const index = products.value.indexOf(row);
      if (index !== -1) {
        products.value.splice(index, 1); // 删除指定索引的数据
        handlePageChange(currentPage.value);
        if (currentPage.value > 1 && currentPageData.value.length === 0) {
          currentPage.value--;
        }
      }
      ElMessage({
        message: '商品已删除',
        type: 'success'
      });
    } else {
      ElMessage({
        message: '删除商品失败',
        type: 'error'
      });
    }}catch (error) {
    if (error !== 'cancel') {
      // console.error('删除商品失败:', error.response ? error.response.data : error.message);
      ElMessage({
        message: '删除商品失败: ' + error.message,
        type: 'error'
      });
    }
  }
};
//删除多个商品
const handleSelectionChange = (selection) => {
  selectedProducts.value = selection;
};
const confirmBatchDelete = () => {
  if (selectedProducts.value.length > 0) {
    const notAllowedToDelete = selectedProducts.value.some(product => product.isOnSale);
    if (notAllowedToDelete) {
      ElMessage({
        message: '所选商品中包含已经出售的商品，无法删除',
        type: 'warning'
      });
    } else {
      confirmDialogVisible.value = true;
    }
  } else {
    ElMessage({
      message: '请选择要删除的商品',
      type: 'warning'
    });
  }
};
const deleteSelectedCommodities = async () => {
  if (selectedProducts.value.length > 0) {
    try {
      const storeId = localStorage.getItem('userId'); 
      const productIds = selectedProducts.value.map(product => product.id);

      // 发送请求到后端批量删除商品
      const response = await axiosInstance.delete('/StoreViewProduct/deleteProducts', {
        params: {
          storeId: storeId
        },
        data: productIds // 传递请求体
      });

      if (response.status === 200) {
        // 从本地商品列表中删除选中的商品
        products.value = products.value.filter(product => !selectedProducts.value.includes(product));
        selectedProducts.value = [];
        confirmDialogVisible.value = false;

        ElMessage({
          message: '选中的商品已删除',
          type: 'success'
        });
      } else {
        ElMessage({
          message: '删除商品失败',
          type: 'error'
        });
      }
    } catch (error) {
      console.error('删除商品失败:', error.response ? error.response.data : error.message);
      ElMessage({
        message: '删除商品失败: ' + error.message,
        type: 'error'
      });
    }
  } else {
    ElMessage({
      message: '请选择要删除的商品',
      type: 'warning'
    });
  }
};
//添加商品
const newProduct = ref({
  name: '',
  categorySys: '',
  categoryInit: '',
  price: null,
  description: '',
  images: [],
  imagesWithText: [],
  Tag: ''
});
// 定义用于瑕疵图文描述的变量
const newImageText = ref('');
const newImageFile = ref(null);
// 定义用于预览的URL
const previewImageURL = ref('');
const previewDefectImageURL = ref('');
//打开添加商品的窗口
const showAddDialog = () => {
  newProduct.value = {
    name: '',
    categorySys: '',
    categoryInit: '',
    price: null,
    description: '',
    images: [], // 存储多个商品图片
    imagesWithText: [],
    Tag: ''
  };
  previewImageURL.value = '';
  previewDefectImageURL.value = '';
  addDialogVisible.value = true;
};
//处理商品图片的添加
const onFileChange = (event) => {
  const files = event.target.files;
  if (files.length > 0) {
    const fileArray = Array.from(files);
    // 将新选择的文件追加到现有的 images 数组中
    newProduct.value.images = [...newProduct.value.images, ...fileArray];
  } else {
    console.log('没有选择文件');
  }
};
//处理瑕疵图片的添加
const handleFileChange = (event) => {
  const file = event.target.files[0];
  if (file) {
    newImageFile.value = file;
  }
};
//处理前端图片的展示
const generateImageURL = (file) => {
  return URL.createObjectURL(file);
};
//处理瑕疵图文的添加
const addImageWithText = () => {
  if (newImageFile.value && newImageText.value.trim() !== '') {
    // 直接使用 File 对象存储到 imagesWithText 数组中
    newProduct.value.imagesWithText.push({ image: newImageFile.value, text: newImageText.value.trim() });
    newImageFile.value = null; // 清空图片文件
    newImageText.value = ''; // 清空文字描述
    // 清空文件输入框的值
    const fileInput = document.getElementById('defect-image-input');
    if (fileInput) {
      fileInput.value = '';
    }
  } else {
    ElMessage.warning('请先选择详细图片并输入描述');
  }
};
//给后端传大分类名称Tag
const updateTag = (selectedSubCategoryId) => {
  // 查找选中的系统分类对应的大标题名字
  const selectedCategory = categories.find(group =>
    group.subCategories.some(item => item.subCategoryId === selectedSubCategoryId)
  );

  // 更新 Tag 字段
  newProduct.value.Tag = selectedCategory ? selectedCategory.largeCategoryName : '';
};
//添加商品总接口
const addNewProduct = async () => {
  // 先验证表单
  if (form.value) {
    form.value.validate(async (valid) => {
      if (valid) {
        const formData = new FormData();
        const storeId = localStorage.getItem('userId');
        if (!storeId) {
          ElMessage.error('未找到有效的 storeId');
          return;
        }
        formData.append('storeId', storeId || ''); // 添加 storeId
        formData.append('ProductName', newProduct.value.name || '');
        formData.append('ProductPrice', newProduct.value.price || '');
        formData.append('Tag', newProduct.value.Tag || ''); // 添加 Tag
        formData.append('SubTag', newProduct.value.categorySys || '');
        formData.append('Description', newProduct.value.description || '');
        formData.append('StoreTag', newProduct.value.categoryInit || '');

        // 上传商品图片文件
        newProduct.value.images.forEach((file, index) => {
          formData.append("ProductImages", file); // 确保是 File 对象
        });

        // 上传瑕疵图片和描述
        newProduct.value.imagesWithText.forEach((item, index) => {
          formData.append(`PicDes[${index}].DetailPic`, item.image); // 确保 item.image 是 File 对象
          formData.append(`PicDes[${index}].Description`, item.text);
        });

        try {
          const response = await axiosInstance.post(`/StoreViewProduct/addProduct`, formData, {
            headers: { 'Content-Type': 'multipart/form-data' },
          });

          if (response.status === 200) {
            fetchProducts();
            ElMessage.success('商品已添加');
            newProduct.value = {
              name: '',
              categorySys: '',
              categoryInit: '',
              price: null,
              description: '',
              images: [],
              imagesWithText: [],
              Tag: ''
            };
            previewImageURL.value = '';
            previewDefectImageURL.value = '';
            addDialogVisible.value = false;
          } else {
            ElMessage.error('添加商品失败');
          }
        } catch (error) {
          console.error('添加商品失败:', error.response ? error.response.data : error.message);
          ElMessage.error('添加商品失败: ' + error.message);
        }
      } else {
        ElMessage.error('请完善表单内容');
      }
    });
  } else {
    ElMessage.error('未找到表单引用');
  }
}

return {
  value,
  products,
  dialogVisible,
  dialogVisibleTwo,
  currentProduct,
  totalProducts,
  currentPageData,
  pageSize,
  preProduct,
  newProduct,
  newImageText,
  addDialogVisible,
  confirmDialogVisible,
  selectedProducts,
  handleCheck,
  handleDelete,
  handlePageChange,
  handleEdit,
  onsubmit,
  showAddDialog,
  addNewProduct,
  handleSelectionChange,
  confirmBatchDelete,
  deleteSelectedCommodities,
  rules,
  form,
  editForm,
  searchName,
  searchcategoryInit,
  fetchProducts,
  viewType,
  handleChange,
  filterProducts,
  filterProductsTag,
  onFileChange,
  addImageWithText,
  handleFileChange,
  removeImage,
  handleFile,
  removeSelectedImage,
  selectedImages,
  handleCancel,
  deletedImages,
  handleDefectFile,
  removeSelectedDefectImage,
  submitDefectUpload,
  removeDefectImage,
  selectedDefectImages,
  deletedDefectImages,
  categories,
  message01,
  updateTag,
  updateTagT,
  generateImageURL,
  limitInputLength
};
}
}
</script>

<style scoped>
.CommodityShow {
  position: fixed;
  top: 10vh;
  left: 150px; 
  right: 0;
  bottom: 0;
  background-color: #DFCDC7  ;
  overflow: auto;
}

.TableContainer {
  margin: 10px;
  margin-top: 0;
  /* height:80%; */
}

.SearchContainer {
  margin-left: 990px;
  display: flex; 
  align-items: center;
  height: 60px;
  margin-right: 10px;
}

#BottomButton {
  display: inline-block;
  margin-right: 20px;
}

.paginationContainer {
  display: inline-block;
}

.SettingPopUp {
  position: fixed;
  z-index: 1;
  left: 0;
  top: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(0,0,0,0.4);
  display: flex;
  justify-content: center;
  align-items: center;
}

.SettingContent {
  background-color: #fefefe;
  color: black;
  display: inline-block;
  padding: 5vh;
  border-radius: 20px;
}

.close {
  color: #aaa;
  float: right;
  font-size: 28px;
  font-weight: bold;
}
 
.close:hover,
.close:focus {
  color: black;
  text-decoration: none;
  cursor: pointer;
} 

.el-button--primary {
    background-color: #a13232;
    border-color: #a13232;
}

.el-button--primary:hover {
    background-color: #8b2b2b;
    border-color: #8b2b2b;
}
.el-button--danger {
    background-color: #a13232;
    border-color: #a13232;
}

.el-button--danger:hover {
    background-color: #8b2b2b;
    border-color: #8b2b2b;
}

.animate__animated.animate__fadeInUp{
	--animate-duration: 1.5s;
}
.animate__animated.animate__fadeInDown{
	--animate-duration: 1.5s;
}
.container-block {
    overflow: hidden;
    border-radius: 15px;
	color: #fff;
	display: inline-block;
	margin: 2rem auto;
	max-width: 60%;
	position: relative;	
	&::before {
		/* 特别修改1 */
        /* 企图用覆盖层的颜色改变原本背景色,并保留原本图案*/
		/* background-color: rgba(59, 69, 109,0.3); */
        background-color: rgba(250, 13, 40, 0.3);
        /* background-color:#82111f; */
		bottom: 0;
		content: '';
		display: block;
		position: absolute;
		top: 0;
		width: 100%;
	}
	&:hover {
		.inner-block:before,
		.slider-top-right:after {
			height: 100%;
		}
		.inner-block:after,
		.slider-top-right:before {
			width: 100%;
		}
	}
	img {
		display: block;
		max-width: 100%;
	}
}

.slider-top-right:before,
.inner-block:after {
	height: 2px;
	transition: width .75s ease;
	width: 0%;
}

.slider-top-right:after,
.inner-block:before {
	height: 0%;
	transition: height .75s ease;
	width: 2px;
}

.inner-block:before,
.inner-block:after,
.slider-top-right:before,
.slider-top-right:after {
	background-color: #fff;
	content: '';
	display: block;
	position: absolute;
}

.inner-block {
	font-size: 2em;
	width: 90%;
	height: 90%;
	position: absolute;
	top: 0;
	left: 0;
	right: 0;
	bottom: 0;
	margin: auto;
	&:before {
		bottom: 0;
		left: 0;
	}
	&:after {
		bottom: 0;
		right: 0;
	}
}

.slider-top-right {
	position: relative;
	width: 100%;
	height: 100%;
	display: flex; /* 使用 Flexbox */
	justify-content: center; /* 水平居中 */
	align-items: center; /* 垂直居中 */
	&:before {
		top: 0;
		left: 0;
	}
	&:after {
		top: 0;
		right: 0;
	}
}
.picture-and-text{
	width: 90%;
	height:90%;
	display: flex; 
	flex-direction: row;
}
.picture{
	width:50%;
	border-radius: 20px;
}
.text{
	/* 华文宋 */
	width: 50%;
	font-family: 'Noto Serif SC', serif;
	font-size: 20px;
	padding: 10px 10px 20px 20px;
	overflow-y: auto; 
	max-height: 100%; /* 限制最大高度为父容器高度 */
	text-align: left;
	
}
/* 特别修改2 */
/* 自定义滚动条样式 */
.text::-webkit-scrollbar {
  width: 5px;
}

.text::-webkit-scrollbar-track {
  background: #f1f1f1;
  border-radius: 10px;
}

.text::-webkit-scrollbar-thumb {
  background: #5f697a;
  border-radius: 10px;
}
h2 {
    font-size: 1.5em;
    /* margin-block-start: 0.83em;
    margin-block-end: 0.83em;
    margin-inline-start: 0px;
    margin-inline-end: 0px; */
    font-weight: bold;
    unicode-bidi: isolate;
    color: #ecdada;
    text-align: center;
  }

  .image-preview {
  display: flex;
  flex-wrap: wrap; /* 使图片在容器宽度不足时自动换行 */
  gap: 10px; /* 图片之间的间距 */
}

.image-item {
  display: flex;
  align-items: center; /* 垂直居中对齐图片 */
}

.image-item img {
  max-width: 100px; /* 控制图片最大宽度 */
  max-height: 100px; /* 控制图片最大高度 */
  object-fit: cover; /* 保持图片比例，并填充容器 */
}


.custom-upload .el-upload-list__item {
  width: 100%;
  height: 100%;
}

.custom-upload .el-upload-list__item-thumbnail {
  width: 100%;
  height: 100%;
  object-fit: cover; /* 确保图片充满整个卡片 */
}

.custom-upload .el-upload-list__item-status {
  display: none; /* 如果需要隐藏其他状态图标 */
}
</style>