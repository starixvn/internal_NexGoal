import React, { Component, Fragment } from "react";
import HorizontalTimeline from 'react-horizontal-timeline';
import moment from 'moment';

export default class MemberTimeline extends Component {
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

    render() {
        const pcis = this.props.pcis;

        return (
            <div className="row">
                <div className="col-sm-2" style={{ height: '70px', lineHeight: '100px' }}>{this.props.memberName}</div>
                <div className="col-sm-10">
                    <div style={{ width: '100%', height: '70px', margin: '0 auto' }} >
                        <HorizontalTimeline
                            index={0}
                            indexClick={(index) => {
                            }}
                            getLabel={this.getLabel}
                            values={pcis.map(pci => pci.createdDate)}
                            fillingMotion={{ stiffness: 150, damping: 25 }}
                        />
                    </div>
                </div>
            </div>
        );
    }
} 