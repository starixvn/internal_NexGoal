import React from 'react';
import { Button, Modal } from 'react-bootstrap';
import moment from 'moment';
import getResource from '../../../resourceHelper';
import { DateFormat } from '../../../constants';

export default function Discipline(props) {
    const disciplines = props.disciplines;
    return (
        <Modal keyboard backdrop="static"
            show={props.isShowModal} onHide={props.onCloseModal}
            dialogClassName="custom-modal modal-md form-performance edit "
        >
            <Modal.Header>
                <Modal.Title>{getResource('Discipline')}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <table className="table table-bordered">
                    <thead>
                        <tr>
                            <th>#</th>
                            <th>Description</th>
                            <th>Effect Date</th>
                        </tr>
                    </thead>
                    <tbody>
                        {
                            disciplines.map((discipline, index) => {
                                return (
                                    <tr>
                                        <td>{index + 1}</td>
                                        <td>{discipline.description}</td>
                                        <td className="text-center">{moment(discipline.effectiveDate).format(DateFormat)}</td>
                                    </tr>
                                );
                            })
                        }

                    </tbody>
                </table>
            </Modal.Body>
            <Modal.Footer>
                <Button className="btn-cancel" onClick={props.onCloseModal}>{getResource('Close')}</Button>
            </Modal.Footer>
        </Modal>
    );
}