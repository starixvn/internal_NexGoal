import React, { Component } from 'react';
import { Modal, Button } from 'react-bootstrap';
import { PerformanceProcess } from '../../../constants';
import PerformanceRecord from '../PerformanceRecord';
import getResource from '../../../resourceHelper';
import PAProcessTab from './PAProcessTab';
import CustomScrollbars from '../CustomScrollbars';

export default class ViewPADetail extends Component {
    constructor(props) {
        super(props);
    }

    render() {
        const pa = this.props.paRecord;
        const paRecord = {
            typeId: 1,
            roleId: PerformanceProcess.Reviewer,
            creator: pa.reviewer,
            createdDate: pa.modifiedDate ? pa.modifiedDate : null,
            comment: pa.comment,
            grade: pa.grade
        };
        const title = getResource('PerformanceAppraisal') + ' OF ' + this.props.employee.nickName;

        return (
            <Modal
                show={this.props.isShowModal} onHide={this.props.onCloseModal}
                dialogClassName="custom-modal modal-md form-performance edit performance-appraisal"
            >
                <Modal.Header>
                    <Modal.Title>{title}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <CustomScrollbars>
                        <PerformanceRecord data={paRecord} isHideProjectTag />
                        <PAProcessTab processes={this.props.paProcesses} />
                    </CustomScrollbars>
                </Modal.Body>
                <Modal.Footer>
                    <Button className="btn-cancel" onClick={this.props.onCloseModal}>{getResource('Close')}</Button>
                </Modal.Footer>
            </Modal>
        );
    }
}