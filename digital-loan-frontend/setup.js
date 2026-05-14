const fs = require('fs');
const path = require('path');

const servicesDir = path.join(__dirname, 'services');
if (!fs.existsSync(servicesDir)) {
    fs.mkdirSync(servicesDir, { recursive: true });
    console.log('Services klasörü oluşturuldu');
}

// customerService.js dosyasını oluştur
const serviceCode = `import apiClient from '../api/axiosConfig';

export const customerService = {
    getSummary: async (customerId) => {
        const response = await apiClient.get(\`/customers/\${customerId}/summary\`);
        return response.data;
    }
};
`;

fs.writeFileSync(path.join(servicesDir, 'customerService.js'), serviceCode);
console.log('customerService.js oluşturuldu');
