import React, { Component, Fragment } from "react";
import { Tooltip, OverlayTrigger } from 'react-bootstrap';

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

    getTooltip = pci => (
        <Tooltip id="tooltip">
            <p>Created by <strong>{pci.creator.nickName}</strong> on {moment(pci.createdDate).format("DD MMM, YYYY")}</p>
            <p><strong>Grade</strong>: {pci.grade ? pci.grade.name : 'N/A'}</p>
            <div style={{ wordWrap: 'break-word' }}>
                {pci.comment}
            </div>
        </Tooltip>
    )

    render() {
        const pcis = this.props.pcis;
        const pci = pcis[0];
        return (
            <div className="row" style={{ marginBottom: '10px', lineHeight: '40px' }}>
                <div className="col-sm-2">{this.props.memberName}</div>
                <div className="col-sm-10">

                    <div className="pa-container" >
                        {pcis.map(pci => (
                            <OverlayTrigger placement="bottom" overlay={this.getTooltip(pci)}>
                                <div className="pa-item">
                                    {pci.grade ? pci.grade.name : 'N/A'}
                                </div>
                            </OverlayTrigger>
                        ))
                        }

                    </div>
                </div>
            </div>
        );
    }
} 