import { defineConfig } from 'vite';

export default defineConfig(() => {
    console.log(`APISERVICE_URL=${process.env.APISERVICE_HTTP}`);
    return {
        server: {
            proxy: {
                '/api': {
                    target: process.env.APISERVICE_HTTPS || process.env.APISERVICE_HTTP,
                    changeOrigin: true,
                    rewrite: (path) => path.replace(/^\/api/, ''),
                    configure: (proxy) => {
                        proxy.on('proxyReq', (proxyReq, req) => {
                            console.log('Sending Request:', req.method, req.url);
                            console.log('Resolved URL:', proxyReq.getHeaders());
                            console.log('Resolved path:', proxyReq.path);
                        })
                    }
                }
            }
        }
    };
});