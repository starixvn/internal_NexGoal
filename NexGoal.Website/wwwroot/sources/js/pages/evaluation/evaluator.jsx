import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import Breadcrumb from '../../shared/components/Breadcrumb';
import Select from 'react-select';
import { Form, FormGroup, ControlLabel, FormControl, Button, Checkbox} from 'react-bootstrap';
import avatar from '../../../styles/images/avatar1.jpg';
const Header = () => (
    <thead>
        <tr>
            <th rowSpan="2"><span className="icon-square-edit-outline"></span></th>
            <th rowSpan="2">Nick Name</th>
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

const Evaluator = () => (
    <table className="table table-bordered assign-permission">
        <Header />
        <tbody>
            <tr>
                <td colSpan="13" className="title">Second Half 2018 (29 Oct 2018 - 29 Nov 2018)</td>
            </tr>
            <tr>
                <td colSpan="13">
                        <input type="checkbox"/><label>Rome</label>
                </td>
            </tr>
            <tr>
                <td className="text-center"><input type="checkbox" /></td>
                <td>Hunter</td>
                <td className="text-center"><span className="grade-enough-safety">ME</span></td>
                <td className="text-center"><span className="grade-enough-safety">ME</span></td>
                <td className="text-center"><span className="icon-comment-text-outline"></span></td>
                <td className="text-center"><span className="grade-enough-safety">ME</span></td>
                <td className="text-center"><span className="icon-comment-text-outline"></span></td>
                <td className="text-center"><span className="grade-enough-safety">ME</span></td>
                <td className="text-center"><span className="icon-square-edit-outline"></span></td>
                <td></td>
                <td></td>
                <td className="text-center"><span className="safety-above"></span></td>
                <td></td>
            </tr>
            <tr>
                <td className="text-center"><input type="checkbox" /></td>
                <td>Raymond</td>
                <td className="text-center"><span className="grade-enough-safety">ME</span></td>
                <td className="text-center"><span className="grade-enough-safety">ME</span></td>
                <td className="text-center"><span className="icon-comment-text-outline"></span></td>
                <td className="text-center"><span className="grade-enough-safety">ME</span></td>
                <td className="text-center"><span className="icon-comment-text-outline"></span></td>
                <td className="text-center"><span className="grade-enough-safety">ME</span></td>
                <td className="text-center"><span className="icon-square-edit-outline"></span></td>
                <td></td>
                <td></td>
                <td className="text-center"><span className="safety"></span></td>
                <td></td>
            </tr>
            <tr>
                <td className="text-center"><input type="checkbox" /></td>
                <td>Oliver</td>
                <td className="text-center"><span className="grade-enough-safety">ME</span></td>
                <td className="text-center"><span className="grade-enough-safety">ME</span></td>
                <td className="text-center"><span className="icon-comment-text-outline"></span></td>
                <td className="text-center"><span className="grade-enough-safety">ME</span></td>
                <td className="text-center"><span className="icon-comment-text-outline"></span></td>
                <td className="text-center"><span className="grade-enough-safety">ME</span></td>
                <td className="text-center"><span className="icon-square-edit-outline"></span></td>
                <td></td>
                <td></td>
                <td className="text-center"><span className="safety-below"></span></td>
                <td></td>
            </tr>
            <tr>
                <td className="text-center"><input type="checkbox" /></td>
                <td>Tim</td>
                <td className="text-center"><span className="grade-enough-safety">ME+</span></td>
                <td className="text-center"><span className="grade-enough-safety">ME+</span></td>
                <td className="text-center"><span className="icon-comment-text-outline"></span></td>
                <td className="text-center"><span className="grade-enough-safety">ME+</span></td>
                <td className="text-center"><span className="icon-comment-text-outline"></span></td>
                <td className="text-center"><span className="grade-enough-safety">ME+</span></td>
                <td className="text-center"><span className="icon-square-edit-outline"></span></td>
                <td></td>
                <td></td>
                <td className="text-center"><span className="safety"></span></td>
                <td></td>
            </tr>
        </tbody>
    </table>
);


export default Evaluator;