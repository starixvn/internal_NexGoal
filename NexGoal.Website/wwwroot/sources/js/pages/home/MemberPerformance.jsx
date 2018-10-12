import React, { Component, Fragment } from "react";
import { renderToStaticMarkup } from 'react-dom/server';
import { Tooltip, OverlayTrigger } from 'react-bootstrap';
import ReactTooltip from 'react-tooltip';

import moment from 'moment';

export default class MemberPerformance extends Component {
    constructor(props) {
        super(props);
    }

    getLabel = (date, index) => {
        let pci = this.props.pcis[index];

        if (pci) {
            return (
                <Fragment>
                    <strong>{pci.creator.nickName}</strong>: <span className="grade-exceptional">{pci.grade.name}</span> ({moment(pci.createdDate).format("DD MMM, YYYY")})
                </Fragment>
            );
        }
    }

    getTooltip = pci => {
        var content = (
            <Fragment>
                <div className="clearfix">
                    <img className="img-responsive pull-right" src="/dist/styles/images/avatar1.jpg" style={{ width: '60px', height: '40px' }} />
                    <p>Created by <strong>{pci.creator.nickName}</strong> on {moment(pci.createdDate).format("DD MMM, YYYY")}</p>
                    <p><strong>Grade</strong>: {pci.grade ? pci.grade.name : 'N/A'}</p>
                    <p>
                        {pci.comment}
                        Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.
                        Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.
                        Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.
                        Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.
                    </p>
                </div>
            </Fragment>
        );
        return renderToStaticMarkup(content);
    }

    render() {
        const pcis = this.props.pcis;
        let width = 100 / pcis.length;
        return (
            <div className="row" style={{ marginBottom: '10px', lineHeight: '30px' }}>
                <div className="col-sm-2"><strong>{this.props.memberName}</strong></div>
                <div className="col-sm-10">
                    <div className="pa-container clearfix">
                        <div className="line"></div>
                        {
                            pcis.map((pci, i) => {
                                let className = "";

                                if (pci.employeeId === pci.creatorId) {
                                    className = "self";
                                } else {
                                    if (pci.creator.roles && pci.creator.roles.includes("PM")) {
                                        className = "PM";
                                    } else {
                                        className = "leader";
                                    }
                                }

                                return (
                                    <div key={pci.performanceRecordId} className="pa-item" style={{ width: `${width}%` }}>
                                        <div className={className} data-for="tooltip" data-tip={this.getTooltip(pci)}>{pci.grade ? pci.grade.name : 'N/A'}</div>
                                    </div>
                                )
                            })
                        }
                    </div>
                </div>

            </div>
        );
    }
} 