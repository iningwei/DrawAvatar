#import "WebRequestPermission.h"
// 引入 SystemConfiguration.framework
#import <SystemConfiguration/SystemConfiguration.h>

@implementation IWebRequestPermission
// 检查当前网络连接状态
BOOL IsConnectedToInternet() {
    BOOL isConnected = NO;
    SCNetworkReachabilityRef reachability = SCNetworkReachabilityCreateWithName(NULL, "www.baidu.com");
    SCNetworkReachabilityFlags flags;
    if (SCNetworkReachabilityGetFlags(reachability, &flags)) {
        isConnected = ((flags & kSCNetworkReachabilityFlagsReachable) != 0);
    }
    CFRelease(reachability);
    return isConnected;
}

// 在应用启动时尝试发起一个简单的网络请求
void TryRequestPermissionIfNeeded() {
    if (IsConnectedToInternet()) {
        NSURL *url = [NSURL URLWithString:@"https://www.baidu.com"];
        NSURLSessionDataTask *dataTask = [[NSURLSession sharedSession] dataTaskWithURL:url completionHandler:^(NSData * _Nullable data, NSURLResponse * _Nullable response, NSError * _Nullable error) {
            // 处理网络请求的结果
        }];
        [dataTask resume];
    }
}
@end
