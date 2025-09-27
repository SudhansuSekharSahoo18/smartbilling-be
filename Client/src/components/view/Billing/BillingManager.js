export const calculateTotalMrp = (billItems) => {
    let sumTotal = 0;
    billItems.forEach(x => sumTotal += x.mrp * x.quantity);
    const value = parseFloat(sumTotal)
    return value.toFixed(2);
};

export const calculateNetAmount = (billItems) => {
    let sumTotal = 0;
    billItems.forEach(x => sumTotal += (100 - x.discountPercentage) * 0.01 * x.mrp * x.quantity);
    const value = parseFloat(sumTotal)
    return value.toFixed(2);
};

export const calculateTotalDiscount = (billItems) => {
    let sumTotal = 0;
    billItems.forEach(x => sumTotal += (x.discountPercentage) * 0.01 * x.mrp * x.quantity);
    const value = parseFloat(sumTotal)
    return value.toFixed(2);
};
