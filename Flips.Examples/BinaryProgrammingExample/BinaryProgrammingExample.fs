﻿module Flips.Examples.BinaryProgrammingExample

open Flips
open Flips.Types
open Flips.SliceMap


let solve () =

    let indexes = [1..9]
    let value = [for i in indexes -> i, 10.0 - (float i)] |> SMap.ofList
    let decisions = 
        DecisionBuilder "Chose" {
            for i in indexes ->
                Boolean
        } |> SMap.ofSeq

    let objExpr = sum (value .* decisions)
    let objective = Objective.create "Max" Maximize objExpr

    let uniqueConstraint = Constraint.create "Unique" (sum (1.0 * decisions) <== 1.0)

    let model =
        Model.create objective
        |> Model.addConstraint uniqueConstraint

    let settings = {
        SolverType = SolverType.CBC
        MaxDuration = 30_000L
        WriteLPFile = Some "ConstraintBuilderExample.lp"
    }
    
    let result = Solver.solve settings model

    printfn "--Result--"
    // Match the result of the call to solve
    // If the model could not be solved it will return a `Suboptimal` case with a message as to why
    // If the model could be solved, it will print the value of the Objective Function and the
    // values for the Decision Variables
    match result with
    | Suboptimal msg -> printfn "Unable to solve. Error: %s" msg
    | Optimal solution ->
        printfn "Objective Value: %f" solution.ObjectiveResult

        for (decision, value) in solution.DecisionResults |> Map.toSeq do
            printfn "Decision: %A\tValue: %f" decision.Name value