export const deleteElementByIndex = (array, index) => {
    return array.filter((element, i) => i !== index);
}