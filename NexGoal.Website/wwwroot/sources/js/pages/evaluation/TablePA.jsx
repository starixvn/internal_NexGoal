import React, { Component } from 'react';
import RowPA from './RowPA';

const Header = () => (
    <thead>
        <tr>
            <th rowSpan="2">#</th>
            <th rowSpan="2">Period Name</th>
            <th rowSpan="2">Start</th>
            <th rowSpan="2">End</th>
            <th rowSpan="2">English Policy</th>
            <th colSpan="2">Employee</th>
            <th colSpan="2">PM</th>
            <th colSpan="2">Evaluator</th>
            <th colSpan="2">Reviewer</th>
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
    <table className="table table-bordered assign-permission">
        <Header />
        <tbody>
            <RowPA />
        </tbody>
    </table>
);

export default TablePA;