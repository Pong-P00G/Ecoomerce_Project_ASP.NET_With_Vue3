// Simple script to test and create products in the API
import axios from 'axios';
import https from 'https';

const api = axios.create({
    baseURL: 'https://localhost:5001/api',
    // For self-signed certificates, we need to use a custom agent
    httpsAgent: new https.Agent({ rejectUnauthorized: false })
});

async function createTestProducts() {
    console.log('Creating test products...');
    
    const products = [
        {
            productName: "Wireless Headphones",
            description: "High-quality wireless headphones with noise cancellation",
            basePrice: 99.99,
            stock: 50,
            minStock: 10,
            supplier: "TechCorp",
            isActive: true,
            featured: true,
            categoryNames: ["Electronics"],
            imageUrls: ["https://images.unsplash.com/photo-1505740420928-5e560c06d30e?w=500&h=500&fit=crop"],
            variants: [
                {
                    sku: "WH-BLK-001",
                    price: 99.99,
                    stockQuantity: 25,
                    isActive: true,
                    options: [
                        { variant: "Color", value: "Black" }
                    ]
                },
                {
                    sku: "WH-WHT-001", 
                    price: 99.99,
                    stockQuantity: 25,
                    isActive: true,
                    options: [
                        { variant: "Color", value: "White" }
                    ]
                }
            ]
        },
        {
            productName: "Smart Watch",
            description: "Fitness tracking smartwatch with heart rate monitor",
            basePrice: 199.99,
            stock: 30,
            minStock: 5,
            supplier: "WearTech",
            isActive: true,
            featured: false,
            categoryNames: ["Electronics", "Wearables"],
            imageUrls: ["https://images.unsplash.com/photo-1523275335682-92da4c34ce4c?w=500&h=500&fit=crop"],
            variants: [
                {
                    sku: "SW-BLK-001",
                    price: 199.99,
                    stockQuantity: 15,
                    isActive: true,
                    options: [
                        { variant: "Color", value: "Black" }
                    ]
                },
                {
                    sku: "SW-SLV-001",
                    price: 199.99,
                    stockQuantity: 15,
                    isActive: true,
                    options: [
                        { variant: "Color", value: "Silver" }
                    ]
                }
            ]
        }
    ];

    for (const product of products) {
        try {
            console.log(`Creating product: ${product.productName}`);
            const response = await api.post('/products', product);
            console.log(`✅ Created: ${response.data.message}`);
        } catch (error) {
            console.error(`❌ Failed to create ${product.productName}:`, error.response?.data || error.message);
        }
    }
}

async function testProductFetch() {
    console.log('\nTesting product fetch...');
    try {
        const response = await api.get('/products?page=1&pageSize=10&isActive=true');
        console.log('✅ Products fetched successfully:');
        console.log(`Total items: ${response.data.totalItems}`);
        console.log(`Items: ${response.data.items.length}`);
        console.log('Sample product:', response.data.items[0]);
    } catch (error) {
        console.error('❌ Failed to fetch products:', error.response?.data || error.message);
    }
}

// Run the tests
createTestProducts()
    .then(() => testProductFetch())
    .then(() => console.log('\n✅ Test completed!'))
    .catch(console.error);