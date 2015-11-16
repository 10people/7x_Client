cd $HOME/Desktop

rm -f ios.ipa

rm -r -f ios.xcarchive

cd /Users/7x_rpg_mac/workspace/xcode_workspace/xcode_qixiong

xcodebuild -scheme Unity-iPhone archive -archivePath $HOME/Desktop/ios.xcarchive

xcodebuild -exportArchive -exportFormat ipa -archivePath "$HOME/Desktop/ios.xcarchive" -exportPath "$HOME/Desktop/ios.ipa" -exportProvisioningProfile "iOSTeam Provisioning Profile: com.youxigu.doujianwushuang.kuaiyong"
