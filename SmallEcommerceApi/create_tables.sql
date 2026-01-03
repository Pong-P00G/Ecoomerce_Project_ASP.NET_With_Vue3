-- Create Products Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[products]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[products] (
        [product_id] INT IDENTITY(1,1) PRIMARY KEY,
        [product_name] NVARCHAR(255) NOT NULL,
        [description] NVARCHAR(MAX) NULL,
        [base_price] DECIMAL(18,2) NOT NULL,
        [sku] NVARCHAR(100) NOT NULL,
        [is_active] BIT NOT NULL DEFAULT 1,
        [featured] BIT NOT NULL DEFAULT 0,
        [created_at] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [updated_at] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT [UQ_products_sku] UNIQUE ([sku])
    );
    PRINT 'Table [products] created successfully.';
END
ELSE
BEGIN
    PRINT 'Table [products] already exists.';
END
GO

-- Create Product Images Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[product_images]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[product_images] (
        [image_id] INT IDENTITY(1,1) PRIMARY KEY,
        [product_id] INT NOT NULL,
        [image_url] NVARCHAR(500) NOT NULL,
        [is_primary] BIT NOT NULL DEFAULT 0,
        [display_order] INT NOT NULL DEFAULT 0,
        CONSTRAINT [FK_product_images_product_id] FOREIGN KEY ([product_id]) 
            REFERENCES [dbo].[products]([product_id]) ON DELETE CASCADE
    );
    PRINT 'Table [product_images] created successfully.';
END
ELSE
BEGIN
    PRINT 'Table [product_images] already exists.';
END
GO

-- Create Product Variants Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[product_variants]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[product_variants] (
        [product_variant_id] INT IDENTITY(1,1) PRIMARY KEY,
        [product_id] INT NOT NULL,
        [sku] NVARCHAR(100) NOT NULL,
        [price] DECIMAL(18,2) NOT NULL,
        [stock_quantity] INT NOT NULL DEFAULT 0,
        [is_active] BIT NOT NULL DEFAULT 1,
        [created_at] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT [FK_product_variants_product_id] FOREIGN KEY ([product_id]) 
            REFERENCES [dbo].[products]([product_id]) ON DELETE CASCADE
    );
    PRINT 'Table [product_variants] created successfully.';
END
ELSE
BEGIN
    PRINT 'Table [product_variants] already exists.';
END
GO

-- Create Variants Table (for variant types like Color, Size, etc.)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[variants]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[variants] (
        [variant_id] INT IDENTITY(1,1) PRIMARY KEY,
        [name] NVARCHAR(100) NOT NULL
    );
    PRINT 'Table [variants] created successfully.';
END
ELSE
BEGIN
    PRINT 'Table [variants] already exists.';
END
GO

-- Create Variant Options Table (for variant values like Red, Blue, Small, Large, etc.)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[variant_options]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[variant_options] (
        [option_id] INT IDENTITY(1,1) PRIMARY KEY,
        [variant_id] INT NOT NULL,
        [option_value] NVARCHAR(100) NOT NULL,
        CONSTRAINT [FK_variant_options_variant_id] FOREIGN KEY ([variant_id]) 
            REFERENCES [dbo].[variants]([variant_id]) ON DELETE CASCADE
    );
    PRINT 'Table [variant_options] created successfully.';
END
ELSE
BEGIN
    PRINT 'Table [variant_options] already exists.';
END
GO

-- Create Product Variant Option Junction Table (many-to-many relationship)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[product_variant_option]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[product_variant_option] (
        [product_variant_id] INT NOT NULL,
        [option_id] INT NOT NULL,
        PRIMARY KEY ([product_variant_id], [option_id]),
        CONSTRAINT [FK_product_variant_option_product_variant_id] FOREIGN KEY ([product_variant_id]) 
            REFERENCES [dbo].[product_variants]([product_variant_id]) ON DELETE CASCADE,
        CONSTRAINT [FK_product_variant_option_option_id] FOREIGN KEY ([option_id]) 
            REFERENCES [dbo].[variant_options]([option_id]) ON DELETE CASCADE
    );
    PRINT 'Table [product_variant_option] created successfully.';
END
ELSE
BEGIN
    PRINT 'Table [product_variant_option] already exists.';
END
GO

PRINT 'All tables created successfully!';
GO

