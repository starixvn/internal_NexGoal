import React, { Component } from 'react';
import RowPA from './RowPA';


const Header = () => (
    <thead>
        <tr>
            <th rowSpan="2">Previous Grade</th>
            <th colSpan="2">Employee</th>
            <th colSpan="2">PM</th>
            <th colSpan="2">Evaluator</th>
            <th colSpan="2">Reviewer</th>
            <th rowSpan="2">English Policy</th>
            <th rowSpan="2">Final Grade</th>
        </tr>
        <tr>
            <th>Grade</th>
            <th>Comment</th>
            <th>Grade</th>
            <th>Comment</th>
            <th>Grade</th>
            <th>Comment</th>
            <th>Grade</th>
            <th>Comment</th>
        </tr>
    </thead>
);


const TablePA = () => (
    <table className="table table-bordered">
        <Header />
        <tbody>
            <tr>
                <td colSpan="11" className="title">Second Half 2018 (29 Oct 2018 - 29 Nov 2018)
                </td>
            </tr>
            <tr>
                <td className="text-center grade-enough-safety">ME</td>
                <td className="text-center"><span className="icon-square-edit-outline"></span></td>
                <td></td>
                <td></td>
                <td></td>
                <td></td>
                <td></td>
                <td></td>
                <td></td>
                <td className="text-center"><span className="safety-above"></span></td>
                <td></td>
            </tr>
        </tbody>
    </table>
);

export default TablePA;