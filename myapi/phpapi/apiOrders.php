<?php
header("Content-Type: application/json");

$host = 'localhost';
$db = 'bakery';
$user = 'root';
$pass = '';
$charset = 'utf8mb4';

$dsn = "mysql:host=$host;dbname=$db;charset=$charset";
$options = [
    PDO::ATTR_ERRMODE => PDO::ERRMODE_EXCEPTION,
    PDO::ATTR_DEFAULT_FETCH_MODE => PDO::FETCH_ASSOC,
    PDO::ATTR_EMULATE_PREPARES => false,
];

$pdo = new PDO($dsn, $user, $pass, $options);

if ($_SERVER['REQUEST_METHOD'] === 'GET') {
    if (isset($_GET['action']) && $_GET['action'] == 'getProducts') {
        $stmt = $pdo->query("SELECT productID, prodName, prodPrice FROM products");
        $products = $stmt->fetchAll();
        echo json_encode($products);
    } else {
        $stmt = $pdo->query("SELECT o.orderID, o.custName, o.custAddress, o.phoneNum, o.quantity, p.prodName, p.prodPrice, (o.quantity * p.prodPrice) AS amountDue 
                             FROM orders o
                             JOIN products p ON o.productID = p.productID");
        $orders = $stmt->fetchAll();
        echo json_encode($orders);
    }
} elseif ($_SERVER['REQUEST_METHOD'] === 'POST') {
    $input = json_decode(file_get_contents('php://input'), true);
    if (isset($input['custName'], $input['custAddress'], $input['phoneNum'], $input['quantity'], $input['productID'])) {
        $sql = "INSERT INTO orders (custName, custAddress, phoneNum, quantity, productID, amountDue) VALUES (?, ?, ?, ?, ?, ?)";
        $stmt = $pdo->prepare($sql);
        $amountDue = $input['quantity'] * $input['prodPrice'];  // Calculate amountDue
        $stmt->execute([$input['custName'], $input['custAddress'], $input['phoneNum'], $input['quantity'], $input['productID'], $amountDue]);
        echo json_encode(['message' => 'Order added successfully']);
    } else {
        echo json_encode(['error' => 'Required fields are missing']);
    }
}
?>
