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
    cogs = ["fun","info"]
    
    bot = commands.Bot(command_prefix=pogfix)
    
    

    @bot.command(pass_context=True)
    async def bank_register(ctx):
        """[Bank] Register a bank account!"""
        if str(ctx.message.author.id) in bank:
            await ctx.send('Sorry, but you already have an account with the Bank of Sony.')
        else:
            bank[str(ctx.message.author.id)] = 100
            await ctx.send('Thank you for registering with the Bank of Sony!\nYour Account ID number is ' + str(ctx.message.author.id) + ' and your account has been activated.\nYour initial balance is ' + str(bank[str(ctx.message.author.id)]) + ' Dosh')

    @bot.command(pass_context=True, hidden=True)
    async def bank_reset(ctx):
        """[Owner] Reset all bank accounts!"""
        if ctx.message.author.id == config.owner:
            bank.clear()
            await ctx.send('All accounts have been wiped!')

    @bot.command(pass_context=True, hidden=True)
    async def ibank_reset(ctx, arg):
        """[Owner] Reset a bank account!"""
        if ctx.message.author.id == config.owner:
            bank.pop(arg, None)
            await ctx.send('Account ID '+str(arg)+' has been wiped.')

    @bot.command(pass_context=True, hidden=True)
    async def dumpbank(ctx):
        """[Debug] Dumps bank dictionary to a file."""
        if ctx.message.author.id == config.owner:
            # with open('dumpbank_buffer.json', 'w') as f:
            #   json.dump(bankb, f)
            #   await ctx.send('Dumped bank bank to file')
            #   f.close()
            with open('dumpbank_dictionary.json', 'w') as f:
                json.dump(bank, f)
                await ctx.send('Dumped bank dictionary to file')
                f.close()

    @bot.command(pass_context=True, hidden=True)
    async def saybank(ctx):
        """[Debug] Replies with bank dictionary."""
        if ctx.message.author.id == config.owner:
            await ctx.send(str(bank))

    @bot.command(pass_context=True, hidden=True)
    async def savebank(ctx):
        """[Bank] Save the bank dictionary to a file."""
        if ctx.message.author.id == config.owner:
            msg = await ctx.send('Saving bank balances.')
            with open('save_bank.json', 'w') as f:
                json.dump(bank, f)
                await msg.edit(content='Saved bank balances to file')
                f.close()

    @bot.command(pass_context=True, hidden=True)
    async def shutdown(ctx):
        """[Owner] Save all files and shutdown."""
        if ctx.message.author.id == config.owner:
            msg = await ctx.send('Saving files.')
            with open('settings_filter.json', 'w') as f:
                json.dump(settings_filter, f)
                f.close()
            with open('save_bank.json', 'w') as f:
                json.dump(bank, f)
                f.close()
            with open('settings_superfilterbans.json', 'w') as f:
                json.dump(settings_superfilterbans, f)
                f.close()
            with open('tags.json', 'w') as f:
                json.dump(tags, f)
                f.close()
            with open('tagso.json', 'w') as f:
                json.dump(tagso, f)
                await msg.edit(content='Saved!\nGood night!\n*Cave Story Theme starts to loop.*')
                f.close()
            print('Logged out.')
            await bot.logout()

    @bot.command(pass_context=True, hidden=True)
    async def set_balance(ctx, arg1, arg2):
        """[Owner] Save the bank and shutdown."""
        if ctx.message.author.id == config.owner:
            bank[str(arg1)] = int(arg2)
            await ctx.send('Set Account ID '+str(arg1)+ '\'s balance to ' + str(arg2) + ' Dosh.')

    @bot.command(pass_context=True)
    async def balance(ctx):
        """[Bank] Check your bank account."""
        await ctx.send('Bank of Sony ATM\nAccount ID ' + str(ctx.message.author.id) + ' bank balance.\nBalance: ' + str(bank[str(ctx.message.author.id)]) + ' Dosh')

    @bot.command(pass_context=True)
    async def slot(ctx, *, arg):
        """[Casino] Play the slots!"""
        if int(arg) <= 9:
            await ctx.send('You have to bet at least 10 Dosh!')
        else:
            if bank[str(ctx.message.author.id)] >= int(arg):
                await ctx.send('The machine takes your Dosh. You pull the lever.')
                bet = int(arg)
                money = bank[str(ctx.message.author.id)]
                number1 = random.randint(0,9)
                number2 = random.randint(0,9)
                number3 = random.randint(0,9)
                await ctx.send('The display says '+str(number1)+' '+str(number2)+' '+str(number3)+'.')
                if number1 == number2 and number2 == number3:
                    await ctx.send('The machine lights up and flashes! You\'ve won the jackpot!\nThe machine spits out '+str(bet*4)+' Dosh')
                    bank[str(ctx.message.author.id)] = money + bet*3
                elif number1 == number2 or number2 == number3:
                    await ctx.send('The machine lights up! You\'ve won!\nThe machine spits out '+str(bet*2)+' Dosh')
                    bank[str(ctx.message.author.id)] = money + bet
                elif number1 == number3:
                    await ctx.send('The machine lights up! You\'ve won!\nThe machine spits out '+str(bet)+' Dosh')
                else:
                    await ctx.send('The machine plays a sad sound. You\'re in the hole '+str(bet)+' Dosh.')
                    bank[str(ctx.message.author.id)] = money - bet
            else:
                await ctx.send('The machine isn\'t stupid! You clearly don\'t have enough money to afford the bet! Lower your wager!')


    @bot.command(pass_context=True)
    async def r34(ctx, arg):
        """[NSFW] Search Rule34.
        Command restricted to NSFW channels."""
        if ctx.message.channel.is_nsfw():
            async with aiohttp.ClientSession() as session:
                async with session.get('https://r34-json-api.herokuapp.com/posts?tags='+str(arg)) as r:
                    if r.status == 200:
                        js = await r.json()
                        rand = random.randint(1,20)
                        jsparse = js[rand]['file_url']
                        await ctx.send(jsparse)
                    else:
                        await ctx.send('Search failed.')
        else:
            await ctx.send("This command is restricted to NSFW channels.")

    
    @bot.command(pass_context=True,hidden=True)
    async def sfunban(ctx, *, arg):
        """[Owner] Unban someone from Superfilter"""
        if ctx.message.author.id == config.owner:
            await ctx.send(str(arg)+' has been unbanned from Superfilter.')
            settings_superfilterbans[arg] = 0

    @bot.command(pass_context=True)
    async def sayfilter(ctx):
        """[Settings] Toggle the filter for the say command."""
        if ctx.message.author.id == ctx.message.guild.owner_id or ctx.message.author.id == config.owner:
            if settings_filter.get(str(ctx.message.guild.id)) == None:
                settings_filter[str(ctx.message.guild.id)] = 1
                await ctx.send('Filtering disabled!')

            elif settings_filter[str(ctx.message.guild.id)] == 0:
                settings_filter[str(ctx.message.guild.id)] = 1
                await ctx.send('Filtering disabled!')

            elif settings_filter[str(ctx.message.guild.id)] == 1:
                settings_filter[str(ctx.message.guild.id)] = 0
                await ctx.send('Filtering enabled!')

        else:
            await ctx.send('Only server owners can set this!')

    @bot.command(pass_context=True, hidden=True)
    async def saysf(ctx):
        """[Debug] Replies with superfilter bans."""
        if ctx.message.author.id == config.owner:
            await ctx.send(str(settings_superfilterbans))

    @bot.command(pass_context=True, hidden=True)
    async def savesf(ctx):
        """[Settings] Save superfilter bans to a file."""
        if ctx.message.author.id == config.owner:
            msg = await ctx.send('Saving superfilter bans.')
            with open('settings_superfilterbans.json', 'w') as f:
                json.dump(settings_filter, f)
                await msg.edit(content='Saved superfilter bans to file')
                f.close()

    @bot.command(pass_context=True, hidden=True)
    async def sayset(ctx):
        """[Debug] Replies with settings."""
        if ctx.message.author.id == config.owner:
            await ctx.send(str(settings_filter))

    @bot.command(pass_context=True, hidden=True)
    async def saveset(ctx):
        """[Settings] Save settings to a file."""
        if ctx.message.author.id == config.owner:
            msg = await ctx.send('Saving settings.')
            with open('settings_filter.json', 'w') as f:
                json.dump(settings_filter, f)
                await msg.edit(content='Saved settings to file')
                f.close()

    @bot.command(pass_context=True, hidden=True)
    async def saytags(ctx):
        """[Debug] Replies with tags."""
        if ctx.message.author.id == config.owner:
            await ctx.send(str(tags))
            await ctx.send(str(tagso))

    @bot.command(pass_context=True, hidden=True)
    async def savetags(ctx):
        """[Settings] Save tags to a file."""
        if ctx.message.author.id == config.owner:
            msg = await ctx.send('Saving tags.')
            with open('tags.json', 'w') as f:
                json.dump(tags, f)
                f.close()
            with open('tagso.json', 'w') as f:
                json.dump(tagso, f)
                await msg.edit(content='Saved tags to file')
                f.close()

  

    @bot.command(pass_context=True)
    async def view(ctx, arg):
        """[Tags] View a tag."""
        if tags.get(arg) == None:
            await ctx.send('No tag exists with that name.')
        else:
            await ctx.send(tags[arg])

    @bot.command(pass_context=True)
    async def create(ctx, arg1, arg2):
        """[Tags] Creates a tag."""
        if tags.get(arg1) != None:
            await ctx.send('A tag already exists with that name.')
        elif any(s in arg1.lower() for s in brfilter.badwords):
            await ctx.send('Your tag contains filtered words.')
        elif any(s in arg2.lower() for s in brfilter.badwords):
            await ctx.send('Your message contains filtered words.')
        else:
            tags[arg1] = arg2
            tagso[arg1] = ctx.message.author.id
            await ctx.send('Tag made!')

    @bot.command(pass_context=True)
    async def delete(ctx, arg):
        """[Tags] Deletes a tag."""
        if tags.get(arg) == None:
            await ctx.send('No tag exists with that name.')
        elif tagso[arg] != ctx.message.author.id:
            await ctx.send('You don\'t own that tag.')
        else:
            tags.pop(arg, None)
            tagso.pop(arg, None)
            await ctx.send('Tag deleted!')

    @bot.command(pass_context=True)
    async def edit(ctx, arg1, arg2):
        """[Tags] Edits a tag."""
        if tags.get(arg1) == None:
            await ctx.send('No tag exists with that name.')
        elif tagso[arg1] != ctx.message.author.id:
            await ctx.send('You don\'t own that tag.')
        else:
            tags[arg1] = arg2
            await ctx.send('Tag edited!')

    if os.path.exists('save_bank.json') == True:
        f = open('save_bank.json')
        bank = json.load(f)
        # print(bank)
        f.close()
    else:
        bank = {}

    if os.path.exists('settings_filter.json') == True:
        f = open('settings_filter.json')
        settings_filter = json.load(f)
        # print(settings_filter)
        f.close()
    else:
        settings_filter = {}
            
    if os.path.exists('tags.json') == True:
        f = open('tags.json')
        tags = json.load(f)
        # print(tags)
        f.close()
    else:
        tags = {}

    if os.path.exists('tagso.json') == True:
        f = open('tagso.json')
        tagso = json.load(f)
        # print(tagso)
        f.close()
    else:
        tagso = {}

    if os.path.exists('settings_superfilterbans.json') == True:
        f = open('settings_superfilterbans.json')
        settings_superfilterbans = json.load(f)
        # print(settings_superfilterbans)
        f.close()
    else:
        settings_superfilterbans = {}

    # print('Bot running.')
    bootsec = time.time()
    for c in cogs: 
        bot.load_extension(c)
    bot.run(config.token)
except KeyboardInterrupt:
    print('Keyboard Interrupt detected!')
    print('Exiting gracefully!')
    with open('settings_filter.json', 'w') as f:
        json.dump(settings_filter, f)
        f.close()
    with open('save_bank.json', 'w') as f:
        json.dump(bank, f)
        f.close()
    with open('tags.json', 'w') as f:
        json.dump(tags, f)
        f.close()
    with open('settings_superfilterbans.json', 'w') as f:
        json.dump(settings_superfilterbans, f)
        f.close()
    with open('tagso.json', 'w') as f:
        json.dump(tagso, f)
        print('Files saved, bot exited.')
        f.close()