import React, { Component, Fragment } from 'react';
import {Tooltip, OverlayTrigger} from 'react-bootstrap';

export default class RowPA extends Component {
    constructor(props) {
        super(props);
        this.renderCellEnglishPolicy = this.renderCellEnglishPolicy.bind(this);
        this.renderCellComment = this.renderCellComment.bind(this);
    }

    renderCellEnglishPolicy() {
        return(
            <td className="text-center"><span className="fa fa-arrow-down"></span></td>
        );
    }
    
    renderCellComment(comment) {
        console.log({comment});
        const tooltip = (
            <Tooltip id="tooltip">
                {comment}
            </Tooltip>
        );
        return (
            <td className="text-center">
               <OverlayTrigger placement="top" overlay={tooltip}>
                    <span className="icon icon-comment-text-outline"></span>
                </OverlayTrigger>
            </td>
        );
    }

    render() {
        const periodData = [
            { 
                periodName: "Second Half 2018", 
                start: "29 Oct 2018",
                end: "29 Oct 2018", 
                englishPolicy: "", 
                employeeGrade: "ME", 
                employeeComment: "N/A", 
                PMGrade: "ME", 
                PMComment: "N/A", 
                evaluatorGrade: "ME", 
                evaluatorComment: "N/A", 
                reviewerGrade: "ME+", 
                reviewerComment: "Faisal test </br> Test 2"
            }, 
            { 
                periodName: "Second Half 2018", 
                start: "29 Oct 2018",
                end: "29 Oct 2018", 
                englishPolicy: "", 
                employeeGrade: "ME", 
                employeeComment: "N/A", 
                PMGrade: "ME", 
                PMComment: "N/A", 
                evaluatorGrade: "ME", 
                evaluatorComment: "test 2", 
                reviewerGrade: "ME+", 
                reviewerComment: "N/A"

            }, 
            { 
                periodName: "Second Half 2018", 
                start: "29 Oct 2018",
                end: "29 Oct 2018", 
                englishPolicy: "", 
                employeeGrade: "ME", 
                employeeComment: "test 3", 
                PMGrade: "ME", 
                PMComment: "N/A", 
                evaluatorGrade: "ME", 
                evaluatorComment: "N/A", 
                reviewerGrade: "ME+", 
                reviewerComment: "N/A"

            }
        ];
        const periodItems = periodData.map((periodItem, index) =>
            <tr key={index}>
                <td className="text-center">{index + 1}</td>
                <td><a href="">{periodItem.periodName}</a></td>
                <td className="text-center">{periodItem.start}</td>
                <td className="text-center">{periodItem.end}</td>
                {this.renderCellEnglishPolicy()}
                <td className="text-center">{periodItem.employeeGrade}</td>
                {this.renderCellComment(periodItem.employeeComment)}
                <td className="text-center">{periodItem.PMGrade}</td>
                {this.renderCellComment(periodItem.PMComment)}
                <td className="text-center">{periodItem.evaluatorGrade}</td>
                {this.renderCellComment(periodItem.evaluatorComment)}
                <td className="text-center">{periodItem.reviewerGrade}</td>
                {this.renderCellComment(periodItem.reviewerComment)}
            </tr>
        );
        return(
            <Fragment>
            {periodItems}
            </Fragment>
        );

    }
}