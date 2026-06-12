#import <Foundation/Foundation.h>

@interface IWebRequestPermission : NSObject
 
#ifdef __cplusplus
extern "C" {
#endif
    BOOL IsConnectedToInternet();
    void TryRequestPermissionIfNeeded();
#ifdef __cplusplus
}
#endif
@end
