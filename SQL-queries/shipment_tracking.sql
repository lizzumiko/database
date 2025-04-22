-- First, remove carrier references from orders
UPDATE orders SET carrier_id = NULL;

-- Now we can safely delete carriers
DELETE FROM carriers;

-- Insert carriers
INSERT INTO carriers (carrier_name, contact_url, contact_phone)
VALUES 
    ('DHL', 'https://www.dhl.com', '+49 228 767 676');

INSERT INTO carriers (carrier_name, contact_url, contact_phone)
VALUES 
    ('UPS', 'https://www.ups.com', '+1 800 742 5877');

-- Verify carriers were inserted
SELECT * FROM carriers ORDER BY carrier_id;

-- Update first order to use DHL
UPDATE orders
SET carrier_id = (SELECT carrier_id FROM carriers WHERE carrier_name = 'DHL'),
    tracking_number = 'DH123456789',
    shipped_date = NOW(),
    order_status = 'Shipped'
WHERE order_id = 1;

-- Update second order to use UPS
UPDATE orders
SET carrier_id = (SELECT carrier_id FROM carriers WHERE carrier_name = 'UPS'),
    tracking_number = 'UPS987654321',
    shipped_date = NOW(),
    order_status = 'Shipped'
WHERE order_id = 2;

-- Verify both orders were updated
SELECT o.order_id,
       o.order_status,
       c.carrier_name,
       o.tracking_number,
       o.shipped_date,
       o.delivered_date
FROM orders o
LEFT JOIN carriers c ON o.carrier_id = c.carrier_id
ORDER BY o.order_id;

-- Mark first order as delivered
UPDATE orders
SET delivered_date = NOW(),
    order_status = 'Delivered'
WHERE order_id = 1;

-- Mark second order as delivered
UPDATE orders
SET delivered_date = NOW(),
    order_status = 'Delivered'
WHERE order_id = 2;

-- Verify final status of both orders
SELECT o.order_id,
       o.order_status,
       c.carrier_name,
       o.tracking_number,
       o.shipped_date,
       o.delivered_date
FROM orders o
LEFT JOIN carriers c ON o.carrier_id = c.carrier_id
ORDER BY o.order_id; 