export const formatDate = (dateStr) => {
    const date = new Date(dateStr);
    const options = {
        year: 'numeric',
        month: 'long',
        day: 'numeric',
        hour: '2-digit',
        minute: '2-digit',
        second: '2-digit',
    };
    return date.toLocaleDateString('en-US', options);
};