#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
 
#ifdef __cplusplus
extern "C" {
#endif
 
void setBrightness(float val) { [UIScreen mainScreen].brightness = val; }

float getBrightness() { return [UIScreen mainScreen].brightness; }

#ifdef __cplusplus
 }
#endif