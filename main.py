try:
    import discord
    import aiohttp
    import config
    import random
    import json
    import time
    import os
    import asyncio
    import brfilter
    from discord.ext import tasks, commands

    pogfix = config.prefix


    bank = {}
    cogs = ['bank','casino','fun','info','nsfw','owner','settings','tags']

    bot = commands.Bot(command_prefix=pogfix)

    async def buildEmbed(title, imgUrl):
        embed = discord.Embed(title = title)
        embed.set_image(url = imgUrl)
        return embed

    async def buildEmbed_basic(title):
        embed = discord.Embed(title = title)
        return embed

    async def bot_load_sfbans():
        if os.path.exists('settings_superfilterbans.json') == True:
            f = open('settings_superfilterbans.json')
            settings_superfilterbans = json.load(f)
            # print(settings_superfilterbans)
            f.close()
        else:
            settings_superfilterbans = {}

        return settings_superfilterbans

    async def bot_save_sfbans(sfbans):
        try:
            with open('settings_superfilterbans.json', 'w') as f:
                json.dump(sfbans, f)
                f.close()
                return 1
        except:
            return 0

    async def bot_load_filter():
        if os.path.exists('settings_filter.json') == True:
            f = open('settings_filter.json')
            settings_filter = json.load(f)
            f.close()
        else:
            settings_filter = {}

        return settings_filter

    async def bot_save_filter(settings_filter):
        try:
            with open('settings_filter.json', 'w') as f:
                json.dump(settings_filter, f)
                f.close()
                return 1
        except:
            return 0

    async def bot_load_bank():
        if os.path.exists('save_bank.json') == True:
            f = open('save_bank.json')
            bank = json.load(f)
            f.close()
        else:
            bank = {}

        return bank

    async def bot_save_bank(bank):
        try:
            with open('save_bank.json', 'w') as f:
                json.dump(bank, f)
                f.close()
                return 1
        except:
            return 0

    async def bot_load_tags():
        if os.path.exists('tags.json') == True:
            f = open('tags.json')
            tags = json.load(f)
            f.close()
        else:
            tags = {}

        return tags

    async def bot_save_tags(tags):
        try:
            with open('tags.json', 'w') as f:
                json.dump(tags, f)
                f.close()
                return 1
        except:
            return 0

    async def bot_load_tagso():
        if os.path.exists('tagso.json') == True:
            f = open('tagso.json')
            tagso = json.load(f)
            f.close()
        else:
            tagso = {}

        return tagso

    async def bot_save_tagso(tagso):
        try:
            with open('tagso.json', 'w') as f:
                json.dump(tagso, f)
                f.close()
                return 1
        except:
            return 0

    if __name__ == '__main__':
        print('Loading json files')
        print('Bot running.')
        for c in cogs:
            bot.load_extension(c)
        bot.run(config.token)

except KeyboardInterrupt:
    print('Keyboard Interrupt detected!')
    print('Bot exited!')
