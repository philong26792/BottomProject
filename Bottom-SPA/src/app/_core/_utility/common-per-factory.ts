export const commonPerFactory = {
    //// local
    serverSentTokenInAppModule: 'localhost:5000',
    linkSentTokenInAppModule: 'localhost:5000/api/auth',
    
    // DB SHC Test
    // serverSentTokenInAppModule: '10.4.0.78:81',
    // linkSentTokenInAppModule: '10.4.0.78:81/api/auth',
    apiUrl: 'http://10.4.0.78:81/api/',       
    urlBackNotLogin: 'https://10.4.0.48:8001/', 
    defaultRackFactory: 'C',                   
    defaultRackWH: 'C4'  
    
    // DB CB Test
    // serverSentTokenInAppModule: '10.4.0.78:71',
    // linkSentTokenInAppModule: '10.4.0.78:71/api/auth',
    // apiUrl: 'http://10.4.0.78:71/api/',       
    // urlBackNotLogin: 'https://10.9.0.35:9000/material-rack', 
    // defaultRackFactory: 'E',                   
    // defaultRackWH: 'E4'        

    // SHC
    // serverSentTokenInAppModule: '10.4.0.48:2021',
    // linkSentTokenInAppModule: '10.4.0.48:2021/api/auth',
    // apiUrl: 'http://10.4.0.48:2021/api/',       // link gọi api product khi build
    // urlBackNotLogin: 'https://10.4.0.48:9000/', // link khi không đăng nhập chuyển đến trang khác
    // defaultRackFactory: 'C',                    // mặc định chọn factory nào trong racklocation main
    // defaultRackWH: 'C4'                         // mặc định chọn WH nào trong racklocation add

    // SPC
    // serverSentTokenInAppModule: '10.10.0.25:9001',
    // linkSentTokenInAppModule: '10.10.0.25:9001/api/auth',
    // apiUrl: 'http://10.10.0.25:9001/api/',
    // urlBackNotLogin: 'https://10.10.0.25/material-rack',
    // defaultRackFactory: 'D',
    // defaultRackWH: 'E4'

    // CB
    // serverSentTokenInAppModule: '10.9.0.9:9001',
    // linkSentTokenInAppModule: '10.9.0.9:9001/api/auth',
    // apiUrl: 'http://10.9.0.9:9001/api/',
    // urlBackNotLogin: 'https://10.9.0.35:9000/material-rack',
    // defaultRackFactory: 'E',
    // defaultRackWH: 'E4'

    // TSH
    // serverSentTokenInAppModule: '10.11.0.22:9001',
    // linkSentTokenInAppModule: '10.11.0.22:9001/api/auth',
    // apiUrl: 'http://10.11.0.22:9001/api/',
    // urlBackNotLogin: 'https://10.11.0.22:9000/',
    // defaultRackFactory: 'U',
    // defaultRackWH: 'U4'
}