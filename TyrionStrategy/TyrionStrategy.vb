Imports System
Imports System.Collections.Generic
Imports TradingMotion.SDKv2.Algorithms
Imports TradingMotion.SDKv2.Algorithms.InputParameters
Imports TradingMotion.SDKv2.Markets.Charts
Imports TradingMotion.SDKv2.Markets.Orders
Imports TradingMotion.SDKv2.Markets.Indicators.Momentum
Imports TradingMotion.SDKv2.Markets.Indicators.StatisticFunctions

Namespace TyrionStrategy

    ''' <summary>
    ''' Tyrion rules:
    '''   * Entry: Price breaks Stochastic %D level
    '''   * Exit: Sets a Take Profit (objective) order based on price standard deviation
    '''   * Filters: None
    ''' </summary>
    Public Class TyrionStrategy
        Inherits Strategy

        Dim stochasticIndicator As StochasticIndicator
        Dim stdDevIndicator As StdDevIndicator

        Dim limitTakeProfitOrder As Order
        
        Public Sub New(ByVal mainChart As Chart, ByVal secondaryCharts As List(Of Chart))
            MyBase.New(mainChart, secondaryCharts)
        End Sub

        ''' <summary>
        ''' Strategy Name
        ''' </summary>
        ''' <returns>The complete Name of the strategy</returns>
        Public Overrides ReadOnly Property Name As String
            Get
                Return "Tyrion Strategy"
            End Get
        End Property

        ''' <summary>
        ''' Security filter that ensures the Position will be closed at the end of the trading session.
        ''' </summary>
        Public Overrides ReadOnly Property ForceCloseIntradayPosition As Boolean
            Get
                Return True
            End Get
        End Property

        ''' <summary>
        ''' Security filter that sets a maximum open position size of 1 contract (either side)
        ''' </summary>
        Public Overrides ReadOnly Property MaxOpenPosition As UInteger
            Get
                Return 1
            End Get
        End Property

        ''' <summary>
        ''' This strategy uses the Advanced Order Management mode
        ''' </summary>
        Public Overrides ReadOnly Property UsesAdvancedOrderManagement As Boolean
            Get
                Return True
            End Get
        End Property

        ''' <summary>
        ''' Strategy Parameter definition
        ''' </summary>
        Public Overrides Function SetInputParameters() As InputParameterList

            Dim parameters As New InputParameterList()

            ' The previous N bars period StdDev indicator will use
            parameters.Add(New InputParameter("StdDev Period", 20))
            ' The number of deviations StdDev indicator will use
            parameters.Add(New InputParameter("StdDev Number of Deviations", 3))

            ' The previous N bars period Stochastic indicator will use
            parameters.Add(New InputParameter("Stochastic Period", 77))

            ' Break level of Stochastic's %D indicator we consider a buy signal
            parameters.Add(New InputParameter("Stochastic %D Buy signal trigger level", 51))

            Return parameters

        End Function

        ''' <summary>
        ''' Initialization method
        ''' </summary>
        Public Overrides Sub OnInitialize()

            log.Debug("TyrionStrategy onInitialize()")

            ' Adding StdDev indicator to strategy 
            ' (see http://stockcharts.com/school/doku.php?id=chart_school:technical_indicators:standard_deviation)
            stdDevIndicator = New StdDevIndicator(Bars.Close, Me.GetInputParameter("StdDev Period"), Me.GetInputParameter("StdDev Number of Deviations"))
            Me.AddIndicator("Std Dev indicator", stdDevIndicator)

            ' Adding Stochastic indicator to strategy 
            ' (see http://stockcharts.com/help/doku.php?id=chart_school:technical_indicators:stochastic_oscillato)
            stochasticIndicator = New StochasticIndicator(Bars.Bars, Me.GetInputParameter("Stochastic Period"))
            Me.AddIndicator("Stochastic", stochasticIndicator)

        End Sub

        ''' <summary>
        ''' Strategy enter/exit/filtering rules
        ''' </summary>
        Public Overrides Sub OnNewBar()

            If Me.GetOpenPosition() = 0 Then

                Dim buySignal As Integer = Me.GetInputParameter("Stochastic %D Buy signal trigger level")

                If stochasticIndicator.GetD()(1) <= buySignal And stochasticIndicator.GetD()(0) > buySignal Then

                    Dim buyOrder As Order = New MarketOrder(OrderSide.Buy, 1, "Entry long")
                    limitTakeProfitOrder = New LimitOrder(OrderSide.Sell, 1, Bars.Close(0) + stdDevIndicator.GetStdDev()(0), "Exit long (take profit stop)")

                    Me.InsertOrder(buyOrder)
                    Me.InsertOrder(limitTakeProfitOrder)

                End If

            End If

        End Sub

    End Class
End Namespace
