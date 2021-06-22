class OnlineWallpaper(OnlineOrLocalCLS):
    def __init__(self, path=None, choice=None, ngchina="no", bingchina="yes", daily_spotlight="yes", alwaysdl_bing="yes"):
        super().__init__()
        if path:
            self._path = path
        else:
            self._path = self.generate_pic_save_path()
        self._image_path = None
        self._ngchina = ngchina
        self._bingchina = bingchina
        self._daily_spotlight = daily_spotlight
        self._alwaysdl_bing = alwaysdl_bing
        self.load_config()
        if choice:
            self.choice = choice
        else:
            self.choice = self.random_choice()
  
    def getPage(self, url):
        user_agent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.97 Safari/537.36 Edg/83.0.478.45"
        headers = { "Referer" : url, "User-Agent":user_agent }
        request = urllib.request.Request(url, headers=headers)
        response = urllib.request.urlopen(request)
        return response.read().decode("utf-8")

class BingChina(OnlineWallpaper):
    def __init__(self, path=None, choice=None, url='https://cn.bing.com/'):
        super().__init__(path=path, choice=choice)
        self._url = url

    # https://www.jianshu.com/p/1e4aa36ec778
    def analyse(self):
        headers = {
        'Accept': 'text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8',
        'Accept-Language': 'en',
        'User-Agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/77.0.3865.90 Safari/537.36',
        'referer': 'https://cn.bing.com/'
        }

        r = requests.get(self._url, headers=headers)

        try:
            url = re.search(r'<div id="bgImgProgLoad" data-ultra-definition-src="(.*?)"', r.text).group(1)
            title = re.search(r'class="sc_light" title="(.*?)"', r.text).group(1)
        except AttributeError:
            print('Wrong parse rules.')
            return None

        image_url = urllib.parse.urljoin(r.url, url)
        water_mark = title
        title = title.replace('/', ' ')
        sep = '_'
        title = re.sub('\W+', sep, title)
        if title[0] == sep:
            title = title[1:]
        if title[-1] == sep:
            title = title[0:-1]
        if re.search(r"\.jpg", image_url, re.I):
            title += ".jpg"
            # WMK: watermark abbr.
        return image_url, title, water_mark
    
    def run(self):
        if self.choice != "bingchina" and self._alwaysdl_bing.lower() != "yes":
            return  "NOT bingchina"
        img_url, img_name, water_mark = self.analyse()
        print("URL:  %s" % img_url)
        prefix = img_name.split(".")[0]
        suffix = img_name.split(".")[1]
        dest_file = prefix + "-WMK." + suffix
        dest_file = os.path.join(self._path, dest_file)
        if not os.path.exists(dest_file):
            self.download_img(img_url, os.path.join(self._path,img_name))
            self.add_water_mark(self._image_path, dest_file, water_mark, font_size=18)
            os.remove(os.path.join(self._path,img_name))

        if self.choice.lower() == "bingchina" and os.path.exists(dest_file):
            self.set_wallpaper(dest_file)
        else:
            print("***** JUST download the picture without setting the wallpaper. *****\n")


class NgChina(OnlineWallpaper):
    def __init__(self, path=None, choice=None, url = u'http://www.ngchina.com.cn/photography/photo_of_the_day/'):
        super().__init__(path=path, choice=choice)
        self._url = url

    def analyse(self):
        url_content = self.getPage(self._url)
        html_suffix = re.search(r'\"/photography/photo_of_the_day/([0-9].+\.html)" title="每日一图：', url_content).group(1)
        photo_of_the_day = self._url + html_suffix
        print("photo_of_the_day:  %s" % photo_of_the_day)
        url_content = self.getPage(photo_of_the_day)
        try:
            item = re.search(r"<img src=\"http://[^>]+\"/>", url_content).group()
            img_url = re.search(r"\"(.+)\"", item).group(1)      
            title = re.search(r"<p class=\"tab_desc\">(.+)</p>", url_content).group(1)
        except Exception as e:
            print(e)
        else:
            if re.search(r"\.jpg", img_url, re.I):
                title += ".jpg"
            img_name = title
        return img_url, img_name
    
    def run(self):
        if self.choice != "ngchina":
            return  "NOT ngchina"     
        img_url, img_name = self.analyse()
        water_mark = img_name
        print("URL:  %s" % img_url)
        prefix = img_name.split(".")[0]
        suffix = img_name.split(".")[1]
        water_mark = prefix
        dest_file = prefix + "-WMK." + suffix
        dest_file = os.path.join(self._path, dest_file)
        if not os.path.exists(dest_file):
            self.download_img(img_url, os.path.join(self._path,img_name))
            self.add_water_mark(self._image_path, dest_file, water_mark, font_size=18)
            os.remove(os.path.join(self._path,img_name))
        if os.path.exists(dest_file):
            self.set_wallpaper(dest_file)


# LOCAL IMAGE SETTER
class WallpaperSetter(OnlineOrLocalCLS):
    def copyto(self, dest_dir=None):
        if self._want2copy.lower() != "yes":
            return "completed"
        self.load_config(img_dir=self._img_dir)
        if not os.path.exists(self._img_dir):
            import errno
            raise FileNotFoundError(errno.ENOENT, os.strerror(errno.ENOENT), self._img_dir)   
        if self._copy_folder != "None":
            dest_dir=self._copy_folder            
        if not os.path.exists(dest_dir):
            os.makedirs(dest_dir)
        files_list = self.images_filter(self._img_dir)
        index = 0
        exists_list = list()
        exists_list_txt = os.path.join(dest_dir, "_existing_file_list.txt")
        for file in files_list:
            index += 1
            basename = os.path.basename(file)
            dest_file = os.path.join(dest_dir, basename)
            
            if os.path.exists(dest_file):
                if not os.path.exists(exists_list_txt):
                    exists_list.append(file)
            
            if not os.path.exists(dest_file):
                shutil.copy(file, dest_file)
                print("[{:>02d}]: copied [ {} ]".format(index, dest_file))
        index = 0
        if exists_list:
            for file in exists_list:
                index += 1
                print("[{:>02d}]: existed [ {} ]".format(index, file))
            exists_list.append('^^^ Some file have been named repeatedly. TIME[{}]. ^^^'.format(time.asctime(time.localtime(time.time()))))
            self.list_converter(exists_list, "to", exists_list_txt)
            print("\n^^^ Some file have been named repeatedly. ^^^")
        print("Copy completed.")
    


    
def local_setter():
    wallpaper_setter = WallpaperSetter(img_dir="D:\jared\erotic\[Wanimal-Wallpaper]")
    print("Starting.")
    try:
        wallpaper_setter.run()
        wallpaper_setter.copyto(dest_dir="C:\\Users\\jared\\Pictures")
    except Exception as e:
        import traceback
        print("{0}\n{1}".format(str(e), traceback.format_exc()))
    else:
        print("Finished.")  

            

if __name__ == "__main__":
    cfg = ConfigParserReader()
    ret = cfg.load_config()
    if ret == "use_photooftheday":
        online_setter()
    elif ret == "use_wallpapersetter":
        local_setter()
    else:
        online_setter()