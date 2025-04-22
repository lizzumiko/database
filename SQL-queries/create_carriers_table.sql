-- Create carriers table
CREATE TABLE carriers (
    carrier_id SERIAL PRIMARY KEY,
    carrier_name VARCHAR(50) NOT NULL,
    contact_url VARCHAR(50),
    contact_phone VARCHAR(50)
);

-- Add carrier_id column to orders table if it doesn't exist
DO $$ 
BEGIN
    IF NOT EXISTS (
        SELECT 1 
        FROM information_schema.columns 
        WHERE table_name = 'orders' 
        AND column_name = 'carrier_id'
    ) THEN
        ALTER TABLE orders ADD COLUMN carrier_id INTEGER REFERENCES carriers(carrier_id);
    END IF;
END $$;

-- Add tracking fields to orders table if they don't exist
DO $$ 
BEGIN
    IF NOT EXISTS (
        SELECT 1 
        FROM information_schema.columns 
        WHERE table_name = 'orders' 
        AND column_name = 'tracking_number'
    ) THEN
        ALTER TABLE orders 
        ADD COLUMN tracking_number VARCHAR(50),
        ADD COLUMN shipped_date TIMESTAMP,
        ADD COLUMN delivered_date TIMESTAMP;
    END IF;
END $$; 