-- Seed data for testing
USE Ecommerce_Project;

-- Insert some test products
INSERT INTO [dbo].[products] ([product_name], [description], [base_price], [sku], [is_active], [featured])
VALUES 
    ('Wireless Headphones', 'High-quality wireless headphones with noise cancellation', 99.99, 'WH-001', 1, 1),
    ('Smart Watch', 'Fitness tracking smartwatch with heart rate monitor', 199.99, 'SW-001', 1, 0),
    ('Bluetooth Speaker', 'Portable bluetooth speaker with deep bass', 79.99, 'BS-001', 1, 0),
    ('USB-C Cable', 'Fast charging USB-C cable 6ft', 19.99, 'USB-001', 1, 0);

-- Insert product images
INSERT INTO [dbo].[product_images] ([product_id], [image_url], [is_primary], [display_order])
VALUES 
    (1, 'https://images.unsplash.com/photo-1505740420928-5e560c06d30e?w=500&h=500&fit=crop', 1, 0),
    (2, 'https://images.unsplash.com/photo-1523275335682-92da4c34ce4c?w=500&h=500&fit=crop', 1, 0),
    (3, 'https://images.unsplash.com/photo-1608043152269-423dbba4e7e1?w=500&h=500&fit=crop', 1, 0),
    (4, 'https://images.unsplash.com/photo-1616594039964-ae9021a400a0?w=500&h=500&fit=crop', 1, 0);

-- Insert variants table data
INSERT INTO [dbo].[variants] ([name])
VALUES 
    ('Color'),
    ('Size');

-- Insert variant options
INSERT INTO [dbo].[variant_options] ([variant_id], [option_value])
VALUES 
    (1, 'Black'),
    (1, 'White'),
    (1, 'Red'),
    (2, 'Small'),
    (2, 'Medium'),
    (2, 'Large');

-- Insert product variants
INSERT INTO [dbo].[product_variants] ([product_id], [sku], [price], [stock_quantity], [is_active])
VALUES 
    (1, 'WH-BLK-001', 99.99, 25, 1),
    (1, 'WH-WHT-001', 99.99, 25, 1),
    (2, 'SW-BLK-001', 199.99, 15, 1),
    (2, 'SW-SLV-001', 199.99, 15, 1),
    (3, 'BS-BLK-001', 79.99, 20, 1),
    (4, 'USB-001', 19.99, 100, 1);

-- Insert product variant options
INSERT INTO [dbo].[product_variant_option] ([product_variant_id], [option_id])
VALUES 
    (1, 1), -- WH-BLK-001 -> Black
    (2, 2), -- WH-WHT-001 -> White
    (3, 1), -- SW-BLK-001 -> Black
    (4, 3), -- SW-SLV-001 -> Red (Silver)
    (5, 1), -- BS-BLK-001 -> Black
    (6, NULL); -- USB-001 -> No variant options

PRINT 'Test data inserted successfully!';