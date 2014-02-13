﻿Imports System
Imports System.Configuration
Imports TradingMotion.SDK.Algorithms.Debug
Imports TradingMotion.SDK.Markets
Imports TradingMotion.SDK.Markets.Charts
Imports TradingMotion.SDK.Markets.Symbols
Imports TradingMotion.SDK.WebServices

Namespace TyrionStrategy

    Module DebugBacktest

        Sub Main()

            ' IMPORTANT INFORMATION
            ' =====================
            '
            ' The purpose of this console application is to allow you to test/debug the Strategy
            ' while you are developing it. 
            ' 
            ' Running this project will perform a DAX 6 month backtest on the Strategy, using 60 min bars.
            ' 
            ' Once the backtest is finished, you will be able to launch the TradingMotionSDKToolkit 
            ' application to see the graphical result.
            ' 
            ' If you want to debug your code you can place breakpoints on the Strategy subclass
            ' and Debug the project.
            ' 
            ' 
            ' REQUIRED CREDENTIALS: Edit your app.config and enter your login/password for accessing the TradingMotion API

            Dim startBacktestDate As DateTime = DateTime.Parse(DateTime.Now.AddMonths(-6).AddDays(-1).ToShortDateString() + " 00:00:00")
            Dim endBacktestDate As DateTime = DateTime.Parse(DateTime.Now.AddDays(-1).ToShortDateString() + " 23:59:59")

            TradingMotionAPIClient.Instance.SetUp("https://www.tradingmotion.com/api/webservice.asmx", ConfigurationManager.AppSettings("TradingMotionAPILogin"), ConfigurationManager.AppSettings("TradingMotionAPIPassword")) 'Enter your TradingMotion credentials on the app.config file
            HistoricalDataAPIClient.Instance.SetUp("http://barserver.tradingmotion.com/WSHistoricalData/webservice.asmx")

            Dim s As TyrionStrategy = New TyrionStrategy(New Chart(SymbolFactory.GetSymbol("FDAX"), BarPeriodType.Minute, 60), Nothing)

            DebugStrategy.RunBacktest(s, startBacktestDate, endBacktestDate)

        End Sub

    End Module

End Namespace